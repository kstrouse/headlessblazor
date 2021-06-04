using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace HeadlessUI.Switch
{
    public partial class HeadlessSwitchLabel
    {
        [Parameter] public bool Passive { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [CascadingParameter] public HeadlessSwitchGroup CascadedGroup { get; set; } = default!;

        [Parameter(CaptureUnmatchedValues = true)] public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

        public HeadlessSwitchGroup Group { get; set; } = default!;

        [MemberNotNull(nameof(Group), nameof(CascadedGroup))]
        public override Task SetParametersAsync(ParameterView parameters)
        {
            //This is here to follow the pattern/example as implmented in Microsoft's InputBase component
            //https://github.com/dotnet/aspnetcore/blob/main/src/Components/Web/src/Forms/InputBase.cs

            parameters.SetParameterProperties(this);

            if (Group == null)
            {
                if (CascadedGroup == null)
                    throw new InvalidOperationException($"You must use {nameof(HeadlessSwitchLabel)} inside an {nameof(HeadlessSwitchGroup)}.");

                Group = CascadedGroup;
            }
            else if (CascadedGroup != Group)
            {
                throw new InvalidOperationException($"{nameof(HeadlessSwitchLabel)} does not support changing the {nameof(HeadlessSwitchGroup)} dynamically.");
            }

            return base.SetParametersAsync(ParameterView.Empty);
        }

        public void HandleClick()
        {
            if (!Passive)
                Group?.ToggleSwitch();
        }
    }
}
