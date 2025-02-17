﻿/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BHE = BH.oM.Environment.Elements;
using BHP = BH.oM.Environment.Fragments;
using BHX = BH.oM.XML;
using BHG = BH.oM.Geometry;

using BH.Engine.Geometry;
using BH.Engine.Environment;

using BH.oM.XML.Enums;

namespace BH.Engine.XML
{
    public static partial class Convert
    {
        public static string ToGBXMLType(this BHE.Panel element, List<List<BHE.Panel>> adjacentSpaces = null, ExportType exportType = ExportType.gbXMLTAS)
        {
            if (adjacentSpaces == null) adjacentSpaces = new List<List<BHE.Panel>>();

            string type = "Air";
            if (element == null)
                return type;

            if (adjacentSpaces.Count == 0)
                type = "Shade";
            else
                type = element.Type.ToGBXML();

            return type;
        }

        public static string ToGBXML(this BHE.PanelType type)
        {
            switch(type)
            {
                case BHE.PanelType.Ceiling:
                    return "Ceiling";
                case BHE.PanelType.Floor:
                case BHE.PanelType.FloorExposed:
                    return "ExposedFloor";
                case BHE.PanelType.FloorInternal:
                    return "InteriorFloor";
                case BHE.PanelType.FloorRaised:
                    return "RaisedFloor";
                case BHE.PanelType.Roof:
                    return "Roof";
                case BHE.PanelType.Shade:
                    return "Shade";
                case BHE.PanelType.SlabOnGrade:
                    return "SlabOnGrade";
                case BHE.PanelType.SolarPanel:
                    return "SolarPanel";
                case BHE.PanelType.UndergroundCeiling:
                    return "UndergroundCeiling";
                case BHE.PanelType.UndergroundSlab:
                    return "UndergroundSlab";
                case BHE.PanelType.UndergroundWall:
                    return "UndergroundWall";
                case BHE.PanelType.Wall:
                case BHE.PanelType.WallExternal:
                    return "ExteriorWall";
                case BHE.PanelType.WallInternal:
                    return "InteriorWall";
                default:
                    return "Air"; //Adiabatic
            }
        }

        public static string ToGBXML(this BHE.OpeningType type)
        {
            switch (type)
            {
                case BHE.OpeningType.Door:
                    return "NonSlidingDoor";
                case BHE.OpeningType.Frame:
                    return "Frame";
                case BHE.OpeningType.Glazing:
                case BHE.OpeningType.Window:
                case BHE.OpeningType.WindowWithFrame:
                case BHE.OpeningType.CurtainWall:
                    return "FixedWindow";
                case BHE.OpeningType.Rooflight:
                case BHE.OpeningType.RooflightWithFrame:
                    return "OperableSkylight";
                case BHE.OpeningType.VehicleDoor:
                    return "VehicleDoor";
                default:
                    return "Air"; //Adiabatic
            }
        }

        public static BHE.PanelType ToBHoMPanelType(this string type)
        {
            switch (type)
            {
                case "Ceiling":
                    return BHE.PanelType.Ceiling;
                case "ExposedFloor":
                    return BHE.PanelType.FloorExposed;
                case "InteriorFloor":
                    return BHE.PanelType.FloorInternal;
                case "RaisedFloor":
                    return BHE.PanelType.FloorRaised;
                case "Roof":
                    return BHE.PanelType.Roof;
                case "Shade":
                    return BHE.PanelType.Shade;
                case "SlabOnGrade":
                    return BHE.PanelType.SlabOnGrade;
                case "SolarPanel":
                    return BHE.PanelType.SolarPanel;
                case "UndergroundCeiling":
                    return BHE.PanelType.UndergroundCeiling;
                case "UndergroundSlab":
                    return BHE.PanelType.UndergroundSlab;
                case "UndergroundWall":
                    return BHE.PanelType.UndergroundWall;
                case "ExteriorWall":
                    return BHE.PanelType.WallExternal;
                case "InteriorWall":
                    return BHE.PanelType.WallInternal;
                default:
                    return BHE.PanelType.Undefined;
            }
        }

        public static BHE.OpeningType ToBHoMOpeningType(this string type)
        {
            switch (type)
            {
                case "NonSlidingDoor":
                    return BHE.OpeningType.Door;
                case "Frame":
                    return BHE.OpeningType.Frame;
                case "FixedWindow":
                    return BHE.OpeningType.Window;
                case "OperableSkylight":
                    return BHE.OpeningType.Rooflight;
                case "VehicleDoor":
                    return BHE.OpeningType.VehicleDoor;
                default:
                    return BHE.OpeningType.Undefined;
            }
        }

    }
}