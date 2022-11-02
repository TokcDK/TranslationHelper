using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THConfig.Groups
{
    public interface ISetting
    {
        /// <summary>
        /// Name of setting for display
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Short description of the setting
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Category name where to place the setting
        /// </summary>
        string Category { get; }
        /// <summary>
        /// Type of value
        /// </summary>
        Type Type { get; }

    }
}
