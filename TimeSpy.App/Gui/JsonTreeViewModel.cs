using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrightIdeasSoftware;
using Newtonsoft.Json.Linq;

namespace TimeSpy
{
    public class JsonTreeViewModel
    {
        public TreeListView ListView;
        public JObject Root;
        public JObject ViewConfig;
        public Func<JToken, string[]> ColumnsGetter;

        public JsonTreeViewModel(JObject root)
        {
            Root = root;
        }

        public void Connect(TreeListView tlv, Func<JToken,string[]> columnsGetter)
        {
            tlv.CanExpandGetter = GetCanExpand;
            tlv.ChildrenGetter = GetChildren;
            tlv.VirtualMode = true;
            ListView = tlv;
            ViewConfig = new JObject();
            ColumnsGetter = columnsGetter;
            UpdateColumns(tlv,ColumnsGetter(Root));
            //tlv.Roots = new object[] { Root };
            tlv.RebuildAll(false);
        }

        private void UpdateColumns(TreeListView tlv, string[] cols)
        {
            tlv.AllColumns = new List<OLVColumn>();
            foreach (var col in cols)
                tlv.AllColumns.Add(
                    CreateTreeListColumn(col,
                        ViewConfig.GetOrAddPath("columns.{0}".Args(col))));
            
            tlv.AllColumns[0].FillsFreeSpace = true;
            tlv.Columns.AddRange(tlv.AllColumns.ToArray());
        }

        public static OLVColumn CreateTreeListColumn(string name, JToken jconfig)
        {
            var col = new OLVColumn(jconfig.AsDyn().caption??name, name);
            jconfig.IfDyn(x=>x.width,(x,v)=>col.Width=v);
            col.AspectGetter = x => ((JToken) x)[name];
            col.AspectPutter = (x, v) => ((JToken) x)[name] = new JValue(v);
            col.AspectToStringConverter = x => string.Format("{0}",x);
            return col;
        }


        private bool GetCanExpand(object x)
        {
            var jt = x as JObject;
            if(jt==null)
                throw new ArgumentException("x is not JObject");
            var ch = jt["children"] as JObject;
            return ch != null && ch.Properties().Count()>0;
        }

        private IEnumerable GetChildren(object x)
        {
            var ch = (JObject) ((JToken) x)["children"];
            return ch.Properties().Where(p => p.HasValues && p.Value is JObject).Select(p=>p.Value);
        }
    }
}
