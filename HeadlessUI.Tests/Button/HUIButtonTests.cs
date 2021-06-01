using Bunit;
using Bunit.JSInterop;
using HeadlessUI.Button;
using Microsoft.AspNetCore.Components.Web;
using Xunit;

namespace HeadlessUI.Tests.Button
{
    public class HUIButtonTests : TestContext
    {
        static IRenderedComponent<HUIButton> component;

        public HUIButtonTests()
        {
            component = SetupButtonComponent();
        }

        private IRenderedComponent<HUIButton> SetupButtonComponent()
        {
            var jsModule = JSInterop.SetupModule("./_content/HeadlessUI/common.js" );
            jsModule.SetupVoid().SetVoidResult();
            return RenderComponent<HUIButton>();
        }

        [Fact]
        public void NewButton_DefaultsToButtonTag()
        {
            var button2 = component.Find("button");
            Assert.NotNull(button2);
        }

        [Fact]
        public void Button_HasRoleOfButton()
        {
            Assert.Contains("role=\"button\"", component.Markup);
        }

        [Fact]
        public void ButtonClick_SpaceKey_EnterKey_AllFireOnClick()
        {
            var clickCount = 0;
            component.SetParametersAndRender(parameters => parameters.Add(p => p.OnClick, (ComponentEventArgs<HUIButton, MouseEventArgs> args) => { clickCount += 1; }));

            var button = component.Find("button");
            button.Click();
            Assert.Equal(1, clickCount);
            button.KeyUp(Key.Space);
            Assert.Equal(2, clickCount);
            button.KeyUp(Key.Enter);
            Assert.Equal(3, clickCount);
        }

        [Fact]
        public void OnClick_WillNotFire_IfButtonDisabled()
        {
            component.SetParametersAndRender(parameters => parameters.Add(p => p.IsEnabled, false));
            var clicked = false;
            component.SetParametersAndRender(parameters => parameters.Add(p => p.OnClick, (ComponentEventArgs<HUIButton, MouseEventArgs> args) => { clicked = true; }));

            var button = component.Find("button");
            button.Click();
            button.KeyUp(Key.Space);
            button.KeyUp(Key.Enter);

            Assert.False(clicked);
        }

        [Fact]
        public void Button_Renders_UnmatchedParameter()
        {
            var param = "criticalInfo";
            var paramValue = 2;
            component.SetParametersAndRender(parameters => parameters.AddUnmatched(param, paramValue));

            Assert.Contains($"{param}=\"{paramValue}\"", component.Markup);
        }

        [Fact]
        public void ButtonHasDisabledAttributes_IfButtonDisabled()
        {
            component.SetParametersAndRender(parameters => parameters.Add(p => p.IsEnabled, false));

            Assert.Contains("aria-disabled", component.Markup);
            Assert.Contains(" disabled ", component.Markup);
        }

        [Fact]
        public void Button_HasAriaLabel()
        {
            var ariaLabel = "test label of button";
            component.SetParametersAndRender(parameters => parameters.Add(p => p.AriaLabel, ariaLabel));

            Assert.Contains($"aria-label=\"{ariaLabel}\"", component.Markup);
        }
    }
}
