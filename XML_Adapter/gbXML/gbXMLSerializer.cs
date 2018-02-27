﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XML = XML_Adapter.gbXML;
using BH.oM.Base;
using BHE = BH.oM.Environmental.Elements;
using BHG = BH.oM.Geometry;
using System.Xml.Serialization;
using BH.Engine.Geometry;
using BH.Engine.Environment;

namespace XML_Adapter.gbXML
{
    public class gbXMLSerializer
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static void Serialize(List<IBHoMObject> bhomObjects, gbXML gbx)
        {

            foreach (IBHoMObject obj in bhomObjects)
            {

                List<BHE.BuildingElementPanel> bHoMPanels = new List<BHE.BuildingElementPanel>();
                List<BHE.BuildingElement> bHoMBuildingElement = new List<BHE.BuildingElement>();

                if (obj.GetType() == typeof(BHE.Space))
                {
                    BHE.Space bHoMSpace = obj as BHE.Space;
                    bHoMPanels.AddRange(bHoMSpace.BuildingElements.Select(x => x.BuildingElementGeometry as BHE.BuildingElementPanel));
                    bHoMBuildingElement.AddRange(bHoMSpace.BuildingElements);
                }


                List<BHE.Space> spaces = bhomObjects.Where(x => x is BHE.Space).Select(x => x as BHE.Space).ToList();

                // Generate gbXMLSurfaces
                if (bHoMPanels != null)
                {
                    List<Surface> srfs = new List<Surface>();
                    for (int i = 0; i < bHoMPanels.Count; i++)
                    {
                        Surface xmlPanel = new Surface();
                        xmlPanel.Name = bHoMPanels[i].Name;
                        //xmlPanel.surfaceType = bHoMPanels[i].ElementType; //Add query
                        xmlPanel.surfaceType = Query.GetSurfaceType(bHoMPanels[i]);
                        xmlPanel.id = "Panel-" + bHoMPanels[i].BHoM_Guid.ToString(); 
                        PlanarGeometry plGeo = new PlanarGeometry();
                        plGeo.id = "PlanarGeometry" + i.ToString();


                        plGeo.PolyLoop = MakePolyloop(bHoMPanels[i].PolyCurve.ControlPoints());

                        xmlPanel.PlanarGeometry = plGeo;

                        // Adjacent Spaces
                        List<AdjacentSpaceId> adspace = new List<AdjacentSpaceId>();
                        //foreach (string adjSpace in bHoMPanels[i].adjSpaces)
                        //{
                        //    AdjacentSpaceId adjId = new AdjacentSpaceId();
                        //    adjId.spaceIdRef = "Space-" + adjSpace;
                        //    adspace.Add(adjId);
                        //}

                        xmlPanel.AdjacentSpaceId = adspace.ToArray();
                        gbx.Campus.Surface.Add(xmlPanel);
                    }

                }

                // Generate gbXMLSpaces
                if (spaces != null)
                {
                    List<XML.Space> xspaces = new List<Space>();
                    foreach (BHE.Space space in spaces)
                    {
                        XML.Space xspace = new XML.Space();
                        xspace.Name = space.Name;
                        xspace.id = "Space-" + space.BHoM_Guid.ToString();
                        List<XML.Polyloop> ploops = new List<Polyloop>();

                        //Just works for polycurves at the moment. ToDo: fix this for all type of curves
                        IEnumerable<BHG.PolyCurve> bePanel = space.BuildingElements.Select(x => x.BuildingElementGeometry.ICurve() as BHG.PolyCurve);
                            

                        foreach (BHG.PolyCurve pline in bePanel)
                        {
                            ploops.Add(MakePolyloop(BH.Engine.Geometry.Query.ControlPoints(pline)));
                            xspace.ShellGeometry.ClosedShell.PolyLoop = ploops.ToArray();
                        }
                        gbx.Campus.Building[0].Space.Add(xspace);
                    }
                }
            }
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/
        private static Polyloop MakePolyloop(List<BHG.Point> pts)
        {

            Polyloop ploop = new Polyloop();
            List<CartesianPoint> cartpoint = new List<CartesianPoint>();
            for (int i = 0; i < pts.Count-1; i++)
            {
                CartesianPoint cpt = new CartesianPoint();
                List<string> coord = new List<string>();
                coord.Add(Math.Round(pts[i].X, 6).ToString());
                coord.Add(Math.Round(pts[i].Y, 6).ToString());
                coord.Add(Math.Round(pts[i].Z, 6).ToString());
                cpt.Coordinate = coord.ToArray();
                cartpoint.Add(cpt);
            }
            ploop.CartesianPoint = cartpoint.ToArray();
            return ploop;
        }
    }
}
