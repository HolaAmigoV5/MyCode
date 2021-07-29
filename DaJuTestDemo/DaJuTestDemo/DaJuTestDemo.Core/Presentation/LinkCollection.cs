using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DaJuTestDemo.Core.Presentation
{
    public class LinkCollection : ObservableCollection<Link>
    {
        public LinkCollection()
        {

        }

        public LinkCollection(IEnumerable<Link> links)
        {
            if (links == null)
                throw new ArgumentNullException("links");

            foreach (var link in links)
            {
                Add(link);
            }
        }
    }
}
