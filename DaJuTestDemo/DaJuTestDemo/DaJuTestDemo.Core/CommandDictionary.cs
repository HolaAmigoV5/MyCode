using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace DaJuTestDemo.Core
{
    /// <summary>
    /// Represents a collection of commands keyed by a uri.
    /// </summary>
    public class CommandDictionary : Dictionary<Uri, ICommand>
    {
    }
}
