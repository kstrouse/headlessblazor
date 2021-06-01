using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System.Threading.Tasks;
using System.Timers;

namespace HeadlessUI.Utilities
{
    public class Transition : ComponentBase
    {
        [Parameter] public RenderFragment<string> ChildContent { get; set; }

        [Parameter] public string Enter { get; set; }
        [Parameter] public string EnterFrom { get; set; }
        [Parameter] public string EnterTo { get; set; }
        [Parameter] public int EnterDuration { get; set; }
        [Parameter] public string Leave { get; set; }
        [Parameter] public string LeaveFrom { get; set; }
        [Parameter] public string LeaveTo { get; set; }
        [Parameter] public int LeaveDuration { get; set; }
        [Parameter] public bool Show { get; set; }

        public string CurrentCssClass { get; private set; }

        [Parameter] public EventCallback<bool> BeforeTransition { get; set; }
        [Parameter] public EventCallback<bool> AfterTransition { get; set; }


        public TransitionState State { get; private set; }
        private bool transitionStarted;
        private System.Timers.Timer transitionTimer;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (State == TransitionState.Visible || State == TransitionState.Hidden || transitionStarted) return;

            transitionStarted = true;
            CurrentCssClass = State == TransitionState.Entering ? $"{Enter} {EnterTo}" : $"{Leave} {LeaveTo}";

            await BeforeTransition.InvokeAsync();

            StartTransitionTimer();
            StateHasChanged();
        }

        private void StartTransitionTimer()
        {
            transitionTimer = new System.Timers.Timer(State == TransitionState.Entering ? EnterDuration : LeaveDuration);
            transitionTimer.Elapsed += OnEndTransition;
            transitionTimer.AutoReset = false;
            transitionTimer.Enabled = true;
        }

        private async void OnEndTransition(object source, ElapsedEventArgs e)
        {
            State = State == TransitionState.Entering ? TransitionState.Visible : TransitionState.Hidden;

            ClearCurrentTransition();

            await AfterTransition.InvokeAsync();

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

        protected override void OnParametersSet()
        {
            if (!StateChangeRequested()) return;

            if (Show)
                InitializeEntering();
            else
                InitializeLeaving();
        }

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
        private bool StateChangeRequested()
        {
            if (Show)
                return State != TransitionState.Visible && State != TransitionState.Entering;
            else
                return State != TransitionState.Hidden && State != TransitionState.Leaving;
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