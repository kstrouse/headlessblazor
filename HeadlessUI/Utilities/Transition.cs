using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Timers;

namespace HeadlessUI.Utilities
{
    public class Transition : ComponentBase
    {
        [CascadingParameter] public TransitionGroup? TransitionGroup { get; set; }

        [Parameter] public RenderFragment<string?>? ChildContent { get; set; }

        [Parameter] public string? Enter { get; set; }
        [Parameter] public string? EnterFrom { get; set; }
        [Parameter] public string? EnterTo { get; set; }
        [Parameter] public int EnterDuration { get; set; }
        [Parameter] public string? Leave { get; set; }
        [Parameter] public string? LeaveFrom { get; set; }
        [Parameter] public string? LeaveTo { get; set; }
        [Parameter] public int LeaveDuration { get; set; }
        [Parameter] public bool Show { get; set; }

        [Parameter] public EventCallback<bool> BeginTransition { get; set; }
        [Parameter] public EventCallback<bool> EndTransition { get; set; }

        public TransitionState State { get; private set; }
        public string? CurrentCssClass { get; private set; }

        private bool transitionStarted;
        private Timer? transitionTimer;
        private bool stateChangeRequested;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (TransitionHasStartedOrCompleted()) return;
            await StartTransition();
        }

        private async Task StartTransition()
        {
            transitionStarted = true;

            //Not sure why this is required when showing but I am guessing it allows blazor to finish the actual
            //dom manipulation of adding the item to the page before we start a new state
            await Task.Yield();

            CurrentCssClass = State == TransitionState.Entering ? $"{Enter} {EnterTo}" : $"{Leave} {LeaveTo}";

            BeginTransition.InvokeAsync();

            StartTransitionTimer();
            StateHasChanged();
        }

        private bool TransitionHasStartedOrCompleted() => State == TransitionState.Visible || State == TransitionState.Hidden || transitionStarted;

        private void StartTransitionTimer()
        {
            transitionTimer = new Timer(State == TransitionState.Entering ? EnterDuration : LeaveDuration);
            transitionTimer.Elapsed += OnEndTransition;
            transitionTimer.AutoReset = false;
            transitionTimer.Enabled = true;
        }

        private void OnEndTransition(object source, ElapsedEventArgs e)
        {
            State = State == TransitionState.Entering ? TransitionState.Visible : TransitionState.Hidden;
            ClearCurrentTransition();

            TransitionGroup?.NotifyEndTransition();
            EndTransition.InvokeAsync();

            StateHasChanged();
        }

        private void ClearCurrentTransition()
        {
            CurrentCssClass = "";
            transitionStarted = false;

            if (transitionTimer == null) return;
            transitionTimer.Dispose();
            transitionTimer = null;
        }

        public override Task SetParametersAsync(ParameterView parameters)
        {
            var currentShowValue = Show;            

            parameters.SetParameterProperties(this);
            
            Show = TransitionGroup?.Show ?? Show;
            stateChangeRequested = currentShowValue != Show;

            return base.SetParametersAsync(ParameterView.Empty);
        }
        protected override void OnParametersSet()
        {
            if (!stateChangeRequested) return;
            
            stateChangeRequested = false;
            
            if (Show)
                InitializeEntering();
            else
                InitializeLeaving();
        }
        protected override void OnInitialized() => TransitionGroup?.RegisterTransition(this);

        private void InitializeEntering()
        {
            ClearCurrentTransition();

            if (EnterDuration == 0)
            {
                State = TransitionState.Visible;
                return;
            }

            State = TransitionState.Entering;
            CurrentCssClass = $"{Enter} {EnterFrom}";
        }
        private void InitializeLeaving()
        {
            ClearCurrentTransition();

            if (LeaveDuration == 0)
            {
                State = TransitionState.Leaving;
                return;
            }

            State = TransitionState.Leaving;
            CurrentCssClass = $"{Leave} {LeaveFrom}";
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (State != TransitionState.Hidden)
                builder.AddContent(0, ChildContent, CurrentCssClass);
        }

        public void Open()
        {
            if (State == TransitionState.Visible || State == TransitionState.Entering) return;
            InitializeEntering();
            StateHasChanged();
        }
        public void Close()
        {
            if (State == TransitionState.Leaving || State == TransitionState.Hidden) return;
            InitializeLeaving();
            StateHasChanged();
        }
        public void Toggle()
        {
            if (State == TransitionState.Visible || State == TransitionState.Entering)
                Close();
            else
                Open();
        }
    }
}