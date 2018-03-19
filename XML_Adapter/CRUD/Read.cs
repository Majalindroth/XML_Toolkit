﻿using System;
using System.Collections.Generic;
using System.Linq;
using XML = XML_Adapter.gbXML;
using BH.oM.Base;
using BHE = BH.oM.Environmental.Elements;
using BHG = BH.oM.Geometry;
using BH.Engine.Geometry;


namespace XML_Adapter.gbXML
{
    public class gbXMLDeserializer
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<BHoMObject> Deserialize(gbXML gbx)
        {
            List<BHoMObject> bhomObjects = new List<BHoMObject>();
            List<XML.Surface> srfs = gbx.Campus.Surface.ToList();
            List<XML.Space> spaces = gbx.Campus.Building[0].Space.ToList();

            // Generate gbXMLSurfaces
            if (srfs != null)
            {
                foreach (Surface srf in srfs)
                {
                    BHE.BuildingElementPanel pan = new BHE.BuildingElementPanel();
                    pan.Name = srf.Name;
                    //pan.Type = srf.surfaceType;
                    pan.CustomData.Add("gbXML-ID", srf.id);
                    pan.PolyCurve = MakeCurveGroup(srf.PlanarGeometry.PolyLoop);
                    bhomObjects.Add(pan);
                }
            }

            // Generate gbXMLSpaces
            if (spaces != null)
            {
                foreach (XML.Space space in spaces)
                {
                    BHE.Space xspace = new BHE.Space();
                    xspace.Name = space.Name;
                    xspace.CustomData.Add("gbXML-ID", space.id);
                    if (4 <= space.ShellGeometry.ClosedShell.PolyLoop.Count())
                    {
                        List<BHE.BuildingElementPanel> bHomPanel = new List<BHE.BuildingElementPanel>();
                        List<BHE.BuildingElement> bHoMBuildingElement = new List<BHE.BuildingElement>();
                        List<BHG.Polyline> plines = new List<BHG.Polyline>();
                        foreach (XML.Polyloop ploop in space.ShellGeometry.ClosedShell.PolyLoop)
                        {
                            if (3 <= ploop.CartesianPoint.Count())
                            {
                                plines.Add(BH.Engine.XML.Convert.ToBHoM(ploop));
                            }    
                        }

                        bHomPanel.AddRange(plines.Select(x => new BHE.BuildingElementPanel { PolyCurve = Create.PolyCurve( new List<BHG.Polyline> { x }) }));  //
                        bHoMBuildingElement.AddRange(bHomPanel.Select(x => new BHE.BuildingElement { BuildingElementGeometry = x }));
                        
                        xspace.BuildingElements = bHoMBuildingElement;
                    }
                    bhomObjects.Add(xspace);
                }
            }
            return bhomObjects;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/
        private static BHG.PolyCurve MakeCurveGroup(Polyloop ploop)
        {
            List<BHG.Point> pts = new List<BH.oM.Geometry.Point>();
            if (1 <= ploop.CartesianPoint.Count())
            {
                foreach (CartesianPoint Cpt in ploop.CartesianPoint)
                {
                    BHG.Point pt = new BHG.Point();
                    pt.X = System.Convert.ToDouble(Cpt.Coordinate[0]);
                    pt.Y = System.Convert.ToDouble(Cpt.Coordinate[1]);
                    pt.Z = System.Convert.ToDouble(Cpt.Coordinate[2]);
                    pts.Add(pt);
                }
                pts.Add((BHG.Point)pts[0].Clone());
            }

            BHG.Polyline pline = Create.Polyline(pts);
            List<BHG.ICurve> crvs = new List<BHG.ICurve>();
            crvs.Add(pline);
            return Create.PolyCurve(crvs);
        }

        /***************************************************/

      
    }
}
