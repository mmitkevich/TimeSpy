using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TColumn = BrightIdeasSoftware.OLVColumn;

namespace TimeSpy
{
    public static class DetailedExtensions
    {
        public static void Configure(this BrightIdeasSoftware.TreeListView tlv, object[] roots, IEnumerable<IDetail> details)
        {
            tlv.CanExpandGetter = x => ((ITreeNode)x).Children.Count > 0;
            tlv.ChildrenGetter = x => ((ITreeNode)x).Children;
            tlv.VirtualMode = true;

            foreach (var d in details)
                tlv.AllColumns.Add(d.CreateTreeListColumn());
            tlv.Columns.AddRange(tlv.AllColumns.ToArray());

            tlv.Roots = roots;
        }

        public static TColumn CreateTreeListColumn(this IDetail d)
        {
            return CreateTreeListColumn(d, d.Name, -1);
        }

        public static Detail<int> Width = new Detail<int>("Width");

        public static TColumn CreateTreeListColumn(this IDetail d, string caption, int width)
        {
            var col = new TColumn(caption, d.Name);
            if (width > 0)
                col.Width = width;
            col.AspectGetter = x => ((Detailed)x).Get(d.Name);
            col.AspectPutter = (x, v) => ((Detailed)x).Set(d.Name, v);
            col.AspectToStringConverter = x => d.ToString(x);
            return col;
        }
    }
}
