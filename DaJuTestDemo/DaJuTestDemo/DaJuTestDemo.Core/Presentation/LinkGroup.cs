

namespace DaJuTestDemo.Core.Presentation
{
    /// <summary>
    /// Represents a named group of links.
    /// </summary>
    public class LinkGroup : Displayable
    {
        private string groupKey;
        /// <summary>
        ///  Gets or sets the key of the group.
        /// </summary>
        public string GroupKey
        {
            get => groupKey;
            set => SetProperty(ref groupKey, value);
        }

        private Link selectedLink;

        /// <summary>
        /// Gets or sets the selected link in this group.
        /// </summary>
        public Link SelectedLink
        {
            get => selectedLink;
            set => SetProperty(ref selectedLink, value);
        }

        private LinkCollection links = new LinkCollection();
        /// <summary>
        /// Gets the links.
        /// </summary>
        public LinkCollection Links
        {
            get => links;
        }
    }
}
