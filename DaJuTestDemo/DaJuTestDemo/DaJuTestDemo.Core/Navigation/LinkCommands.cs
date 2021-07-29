using System.Windows.Input;

namespace DaJuTestDemo.Core
{
    /// <summary>
    /// The routed link commands.
    /// </summary>
    public static class LinkCommands
    {
        /// <summary>
        /// Gets the navigate link routed command.
        /// </summary>
        public static RoutedUICommand NavigateLink { get; } = new RoutedUICommand("Navigate link", "NavigateLink", typeof(LinkCommands));
    }
}
