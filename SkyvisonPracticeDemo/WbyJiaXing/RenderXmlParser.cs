using i3dRenderEngine;
using System;
using System.Xml;

namespace WbyJiaXing
{
    public class RenderXmlParser
    {
        private static ITextSymbol GetTextSymbol(XmlNode symbolNode)
        {
            if (symbolNode == null)
            {
                return null;
            }
            string value = symbolNode.Attributes["DrawLine"].Value;
            string value2 = symbolNode.Attributes["MaxVisualDistance"].Value;
            string value3 = symbolNode.Attributes["MinVisualDistance"].Value;
            string value4 = symbolNode.Attributes["Priority"].Value;
            string s = "0";
            string s2 = "0";
            if (symbolNode.Attributes["PivotOffsetX"] != null)
            {
                s = symbolNode.Attributes["PivotOffsetX"].Value;
                s2 = symbolNode.Attributes["PivotOffsetY"].Value;
            }
            string value5 = symbolNode.Attributes["PivotAlignment"].Value;
            string value6 = symbolNode.Attributes["VerticalOffset"].Value;
            ITextSymbol textSymbol = new TextSymbol();
            textSymbol.DrawLine = (value.ToLower() == "true");
            textSymbol.MaxVisualDistance = double.Parse(value2);
            textSymbol.MinVisualDistance = double.Parse(value3);
            textSymbol.MarginWidth = int.Parse(s);
            textSymbol.MarginHeight = int.Parse(s2);
            textSymbol.Priority = int.Parse(value4);
            textSymbol.PivotAlignment = (i3dPivotAlignment)System.Enum.Parse(typeof(i3dPivotAlignment), value5);
            textSymbol.VerticalOffset = double.Parse(value6);
            XmlNode xmlNode = symbolNode.SelectSingleNode("TextAttribute");
            if (xmlNode != null)
            {
                string value7 = xmlNode.Attributes["TextColor"].Value;
                string value8 = xmlNode.Attributes["Font"].Value;
                string value9 = xmlNode.Attributes["TextSize"].Value;
                string value10 = xmlNode.Attributes["BackgroundColor"].Value;
                string value11 = xmlNode.Attributes["OutlineColor"].Value;
                string value12 = xmlNode.Attributes["Bold"].Value;
                string value13 = xmlNode.Attributes["Italic"].Value;
                string value14 = xmlNode.Attributes["Underline"].Value;
                textSymbol.TextAttribute = new TextAttribute
                {
                    BackgroundColor = uint.Parse(value10),
                    TextColor = uint.Parse(value7),
                    Font = value8,
                    OutlineColor = uint.Parse(value11),
                    TextSize = int.Parse(value9),
                    Bold = bool.Parse(value12),
                    Italic = bool.Parse(value13),
                    Underline = bool.Parse(value14)
                };
            }
            return textSymbol;
        }

        public ITextRender Xml2TextRender(XmlDocument xmlDoc)
        {
            XmlNode textRenNode = this.GetNode(xmlDoc, "FeatureLayer/TextRender");
            if (textRenNode == null)
            {
                return null;
            }

            ITextRender result = null;
            string value = textRenNode.Attributes["Expression"].Value;
            bool dynamicPlacement = false;
            bool minimizeOverlap = false;
            if (textRenNode.Attributes["DynamicPlacement"] != null)
            {
                dynamicPlacement = bool.Parse(textRenNode.Attributes["DynamicPlacement"].Value);
            }
            if (textRenNode.Attributes["MinimizeOverlap"] != null)
            {
                minimizeOverlap = bool.Parse(textRenNode.Attributes["MinimizeOverlap"].Value);
            }
            string value2 = textRenNode.Attributes["TextRenderType"].Value;
            if (value2 == i3dRenderType.i3dRenderSimple.ToString())
            {
                XmlNode xmlNode = textRenNode.SelectSingleNode("TextSymbol");
                if (xmlNode != null)
                {
                    result = new SimpleTextRender
                    {
                        Symbol = GetTextSymbol(xmlNode),
                        Expression = value,
                        MinimizeOverlap = minimizeOverlap,
                        DynamicPlacement = dynamicPlacement
                    };
                }
            }
            else
            {
                XmlNodeList xmlNodeList = textRenNode.SelectNodes("ValueMap/TextScheme");
                if (xmlNodeList != null && xmlNodeList.Count > 0)
                {
                    IValueMapTextRender valueMapTextRender = new ValueMapTextRender();
                    foreach (XmlNode xmlNode2 in xmlNodeList)
                    {
                        ITextRenderScheme textRenderScheme = new TextRenderScheme();
                        XmlNode xmlNode3 = xmlNode2.SelectSingleNode("RenderRule");
                        if (xmlNode3 != null)
                        {
                            string value3 = xmlNode3.Attributes["RuleType"].Value;
                            string value4 = xmlNode3.Attributes["LookUpField"].Value;
                            if (value3 == i3dRenderRuleType.i3dRenderRuleUniqueValues.ToString())
                            {
                                IUniqueValuesRenderRule uniqueValuesRenderRule = new UniqueValuesRenderRule();
                                string value5 = xmlNode3.Attributes["UniqueValue"].Value;
                                uniqueValuesRenderRule.LookUpField = value4;
                                uniqueValuesRenderRule.AddValue(value5);
                                textRenderScheme.AddRule(uniqueValuesRenderRule);
                            }
                            else
                            {
                                IRangeRenderRule rangeRenderRule = new RangeRenderRule();
                                string value6 = xmlNode3.Attributes["IncludeMax"].Value;
                                string value7 = xmlNode3.Attributes["IncludeMin"].Value;
                                string value8 = xmlNode3.Attributes["MaxValue"].Value;
                                string value9 = xmlNode3.Attributes["MinValue"].Value;
                                rangeRenderRule.LookUpField = value4;
                                rangeRenderRule.IncludeMax = (value6.ToLower() == "true");
                                rangeRenderRule.IncludeMin = (value7.ToLower() == "true");
                                rangeRenderRule.MaxValue = double.Parse(value8);
                                rangeRenderRule.MinValue = double.Parse(value9);
                                textRenderScheme.AddRule(rangeRenderRule);
                            }
                        }
                        XmlNode xmlNode4 = xmlNode2.SelectSingleNode("TextSymbol");
                        if (xmlNode4 != null)
                        {
                            textRenderScheme.Symbol = GetTextSymbol(xmlNode4);
                        }
                        valueMapTextRender.AddScheme(textRenderScheme);
                    }
                    valueMapTextRender.Expression = value;
                    valueMapTextRender.MinimizeOverlap = minimizeOverlap;
                    valueMapTextRender.DynamicPlacement = dynamicPlacement;
                    result = valueMapTextRender;
                }
            }
            return result;
        }

        public IGeometryRender Xml2GeoRender(XmlDocument xmlDoc, ref double maxVisibleDistance)
        {
            XmlNode flayer = this.GetNode(xmlDoc, "/FeatureLayer");
            XmlNode geoRenNode = this.GetNode(xmlDoc, "/FeatureLayer/GeometryRender");
            IGeometryRender result;
            if (geoRenNode == null)
            {
                result = null;
            }
            else
            {
                string renType = geoRenNode.Attributes["RenderType"].Value;
                IGeometryRender geoRen;
                if (renType.Equals("i3dRenderSimple"))
                {
                    ISimpleGeometryRender simpleGeoRen = new SimpleGeometryRender();
                    XmlNode geoSymNode = geoRenNode.FirstChild;
                    IGeometrySymbol geoSym = this.Xml2GeoSym(geoSymNode);
                    if (geoSym != null)
                    {
                        simpleGeoRen.Symbol = geoSym;
                    }
                    geoRen = simpleGeoRen;
                }
                else
                {
                    XmlNode vmpNode = this.GetNode(xmlDoc, "/FeatureLayer/GeometryRender/ValueMap");
                    if (vmpNode == null || vmpNode.ChildNodes.Count <= 0)
                    {
                        result = null;
                        return result;
                    }
                    IValueMapGeometryRender valMapRender = new ValueMapGeometryRender();
                    string lookupName = string.Empty;
                    for (int i = 0; i < vmpNode.ChildNodes.Count; i++)
                    {
                        XmlNode schmNode = vmpNode.ChildNodes[i];
                        if (schmNode != null)
                        {
                            XmlNode ruleNode = schmNode.ChildNodes[0];
                            if (ruleNode != null)
                            {
                                i3dRenderRuleType ruleType = (i3dRenderRuleType)System.Enum.Parse(typeof(i3dRenderRuleType), ruleNode.Attributes["RuleType"].Value);
                                IRenderRule RndRule;
                                if (ruleType == i3dRenderRuleType.i3dRenderRuleUniqueValues)
                                {
                                    UniqueValuesRenderRule unqValueRn = new UniqueValuesRenderRule();
                                    string value = ruleNode.Attributes["UniqueValue"].Value;
                                    unqValueRn.AddValue(value);
                                    RndRule = unqValueRn;
                                }
                                else
                                {
                                    IRangeRenderRule rangeRn = new RangeRenderRule();
                                    rangeRn.IncludeMax = bool.Parse(ruleNode.Attributes["IncludeMax"].Value);
                                    rangeRn.IncludeMin = bool.Parse(ruleNode.Attributes["IncludeMin"].Value);
                                    rangeRn.MaxValue = double.Parse(ruleNode.Attributes["MaxValue"].Value);
                                    rangeRn.MinValue = double.Parse(ruleNode.Attributes["MinValue"].Value);
                                    RndRule = rangeRn;

                                }
                                RndRule.LookUpField = ruleNode.Attributes["LookUpField"].Value;

                                lookupName = RndRule.LookUpField;
                                XmlNode symNode = schmNode.ChildNodes[1];
                                if (symNode != null)
                                {
                                    IGeometrySymbol geoSym = this.Xml2GeoSym(symNode);
                                    if (geoSym != null && RndRule != null)
                                    {
                                        IGeometryRenderScheme geoRnSchm = new GeometryRenderScheme();
                                        geoRnSchm.Symbol = geoSym;
                                        geoRnSchm.AddRule(RndRule);
                                        valMapRender.AddScheme(geoRnSchm);
                                    }
                                }
                            }
                        }
                    }
                    IGeometryRenderScheme defScheme = new GeometryRenderScheme();
                    ISurfaceSymbol symbol = new SurfaceSymbol();
                    defScheme.Symbol = symbol;
                    defScheme.AddRule(new UniqueValuesRenderRule
                    {
                        LookUpField = lookupName,
                        Otherwise = true
                    });
                    valMapRender.AddScheme(defScheme);
                    geoRen = valMapRender;
                }
                if (geoRen != null)
                {
                    string heightStyle = geoRenNode.Attributes["HeightStyle"].Value;
                    i3dHeightStyle style = (i3dHeightStyle)System.Enum.Parse(typeof(i3dHeightStyle), heightStyle);
                    geoRen.HeightStyle = style;

                    string groupField = geoRenNode.Attributes["GroupField"].Value;
                    geoRen.RenderGroupField = groupField;

                    if (double.TryParse(geoRenNode.Attributes["HeightOffset"].Value, out double heightOffset))
                    {
                        geoRen.HeightOffset = heightOffset;
                    }
                }
                result = geoRen;
            }
            return result;
        }

        private IGeometrySymbol Xml2GeoSym(XmlNode geoSymNode)
        {
            IGeometrySymbol geoSym = null;
            if (geoSymNode != null)
            {
                string strSymType = geoSymNode.Attributes["GeometryType"].Value;
                if (!strSymType.StartsWith("i3dGeoSymbol"))
                {
                    if (strSymType == "Polygon")
                        strSymType = "i3dGeoSymbol3DPolygon";
                    else if (strSymType == "Polyline")
                        strSymType = "i3dGeoSymbolCurve";
                    else
                        strSymType = "i3dGeoSymbol" + strSymType;
                }
                i3dGeometrySymbolType SymType = (i3dGeometrySymbolType)System.Enum.Parse(typeof(i3dGeometrySymbolType), strSymType);
                switch (SymType)
                {
                    case i3dGeometrySymbolType.i3dGeoSymbolPoint:
                        geoSym = new SimplePointSymbol();
                        break;
                    case i3dGeometrySymbolType.i3dGeoSymbolImagePoint:
                        geoSym = new ImagePointSymbol();
                        break;
                    case i3dGeometrySymbolType.i3dGeoSymbolModelPoint:
                        geoSym = new ModelPointSymbol();
                        break;
                    case i3dGeometrySymbolType.i3dGeoSymbolCurve:
                        geoSym = new CurveSymbol();
                        break;
                    case i3dGeometrySymbolType.i3dGeoSymbolSurface:
                        geoSym = new SurfaceSymbol();
                        break;
                    case i3dGeometrySymbolType.i3dGeoSymbolSolid:
                        geoSym = new SolidSymbol();
                        break;
                    case i3dGeometrySymbolType.i3dGeoSymbol3DPolygon:
                        geoSym = new Polygon3DSymbol();
                        break;
                }

                this.SetAttr2(geoSymNode, geoSym);

            }
            return geoSym;
        }

        private void SetAttr2(XmlNode node, IGeometrySymbol syb)
        {
            switch (syb.SymbolType)
            {
                case i3dGeometrySymbolType.i3dGeoSymbolPoint:
                case i3dGeometrySymbolType.i3dGeoSymbolImagePoint:

                case i3dGeometrySymbolType.i3dGeoSymbolModelPoint:

                case i3dGeometrySymbolType.i3dGeoSymbolSurface:

                case i3dGeometrySymbolType.i3dGeoSymbolSolid:
                    throw new NotImplementedException();
                case i3dGeometrySymbolType.i3dGeoSymbol3DPolygon:
                    Polygon3DSymbol polygonSym = syb as Polygon3DSymbol;
                    if (node.Attributes["Color"] != null)
                        polygonSym.Color = uint.Parse(node.Attributes["Color"].Value);
                    if (node.Attributes["EnableLight"] != null)
                        polygonSym.EnableLight = bool.Parse(node.Attributes["EnableLight"].Value);
                    if (node.Attributes["Height"] != null)
                        polygonSym.Height = double.Parse(node.Attributes["Height"].Value);
                    if (node.Attributes["ImageName"] != null)
                        polygonSym.ImageName = node.Attributes["ImageName"].Value;
                    if (node.Attributes["RepeatLengthU"] != null)
                        polygonSym.RepeatLengthU = float.Parse(node.Attributes["RepeatLengthU"].Value);
                    if (node.Attributes["RepeatLengthV"] != null)
                        polygonSym.RepeatLengthV = float.Parse(node.Attributes["RepeatLengthV"].Value);
                    if (node.Attributes["Rotation"] != null)
                        polygonSym.Rotation = float.Parse(node.Attributes["Rotation"].Value);
                    if (node.Attributes["Script"] != null)
                        polygonSym.Script = node.Attributes["Script"].Value;
                    break;

                case i3dGeometrySymbolType.i3dGeoSymbolCurve:
                    var curveSym = syb as CurveSymbol;
                    if (node.Attributes["Color"] != null)
                        curveSym.Color = uint.Parse(node.Attributes["Color"].Value);
                    if (node.Attributes["Width"] != null)
                        curveSym.Width = float.Parse(node.Attributes["Width"].Value);
                    if (node.Attributes["ImageName"] != null)
                        curveSym.ImageName = node.Attributes["ImageName"].Value;
                    if (node.Attributes["RepeatLength"] != null)
                        curveSym.RepeatLength = float.Parse(node.Attributes["RepeatLength"].Value);
                    break;
            }
        }

        private void SetAttr(XmlNode node, object obj)
        {
            try
            {
                System.Type type = obj.GetType();
                System.Reflection.PropertyInfo[] props = type.GetProperties();
                for (int i = 0; i < node.Attributes.Count; i++)
                {
                    string name = node.Attributes[i].Name;
                    if (!string.IsNullOrEmpty(name))
                    {
                        for (int j = 0; j < props.Length; j++)
                        {
                            if (props[j].CanWrite && props[j].Name.Equals(name))
                            {
                                System.Type propType = props[j].PropertyType;
                                try
                                {
                                    if (propType == typeof(uint))
                                    {
                                        props[j].SetValue(obj, uint.Parse(node.Attributes[i].Value), null);
                                    }
                                    else if (propType == typeof(int))
                                    {
                                        props[j].SetValue(obj, int.Parse(node.Attributes[i].Value), null);
                                    }
                                    else if (propType == typeof(double))
                                    {
                                        props[j].SetValue(obj, double.Parse(node.Attributes[i].Value), null);
                                    }
                                    else if (propType == typeof(string))
                                    {
                                        props[j].SetValue(obj, node.Attributes[i].Value, null);
                                    }
                                    else if (propType == typeof(bool))
                                    {
                                        props[j].SetValue(obj, bool.Parse(node.Attributes[i].Value), null);
                                    }
                                    else if (propType.BaseType == typeof(System.Enum))
                                    {
                                        props[j].SetValue(obj, System.Enum.Parse(propType, node.Attributes[i].Value), null);
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private XmlNodeList GetNodeList(XmlDocument xmlDoc, string path)
        {
            XmlNodeList result;
            try
            {
                XmlNodeList nodes = xmlDoc.SelectNodes(path);
                if (nodes == null)
                {
                    result = null;
                }
                else
                {
                    result = nodes;
                }
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        private XmlNode GetNode(XmlDocument xmlDoc, string path)
        {
            XmlNode result;
            try
            {
                XmlNode node = xmlDoc.SelectSingleNode(path);
                if (node == null)
                {
                    result = null;
                }
                else
                {
                    result = node;
                }
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        public uint ColorParse(string strColor)
        {
            uint color;
            try
            {
                color = uint.Parse(strColor);
            }
            catch (Exception e_0E)
            {
                string sColor = "0x" + strColor;
                color = System.Convert.ToUInt32(sColor, 16);
            }
            return color;
        }

        public XmlDocument LoadXmlDocument(string xmlInfo)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(xmlInfo);
            return LoadXmlDocument(buffer);
        }

        public XmlDocument LoadXmlDocument(byte[] buf)
        {
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream(buf))
            {
                XmlDocument fcDoc = new XmlDocument();
                fcDoc.Load(stream);
                stream.Close();
                return fcDoc;
            }
        }
    }
}
