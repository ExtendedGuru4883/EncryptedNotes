using Client.Models.Forms;
using Microsoft.AspNetCore.Components;

namespace Client.Shared;

public partial class AuthForm : ComponentBase
{
    [Parameter] public required string Name { get; set; }
    [Parameter] public required AuthFormModel Model { get; set; }
    [Parameter] public required bool IsLoading { get; set; }
    [Parameter] public required List<string> Errors { get; set; }
    [Parameter] public required EventCallback OnSubmit  { get; set; }

    private Task HandleSubmit() => OnSubmit.InvokeAsync();
}