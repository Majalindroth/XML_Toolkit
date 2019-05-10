/*
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
using BH.oM.XML;
using BH.oM.Base;
using BH.oM.Environment.Elements;
using BHP = BH.oM.Environment.Fragments;
using BHG = BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.Engine.Environment;

using BH.Engine.XML;
using BH.oM.XML.Enums;

namespace BH.Adapter.XML
{
    public partial class GBXMLSerializer
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static void Serialize<T>(IEnumerable<T> bhomObjects, BH.oM.XML.GBXML gbx, ExportType exportType, ExportDetail exportDetail, string filePath, string projectName) where T : IObject
        {
            switch (exportDetail)
            {
                case ExportDetail.Full:
                    SerializeCollectionFull(bhomObjects as dynamic, gbx, exportType);
                    break;
                case ExportDetail.BuildingShell:
                    SerializeBuildingShell(bhomObjects as dynamic, gbx, exportType);
                    break;
                case ExportDetail.IndividualSpaces:
                    SerializeSpaces(bhomObjects, exportType, filePath, projectName);
                    break;
                default:
                    throw new NotImplementedException("That option has not been implemented");
            }

            // Document History                          
            DocumentHistory DocumentHistory = new DocumentHistory();
            DocumentHistory.CreatedBy.Date = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            gbx.DocumentHistory = DocumentHistory;
        }

        public static void SerializeSpaces<T>(IEnumerable<T> objects, ExportType exportType, string filePath, string projectName) where T : IObject
        {
            SerializeBuildingSpaces(objects as dynamic, exportType, filePath, projectName);
        }

        private static void SerializeCollectionFull(IEnumerable<BH.oM.XML.Environment.DocumentBuilder> documents, GBXML gbx, ExportType exportType)
        {
            foreach (BH.oM.XML.Environment.DocumentBuilder db in documents)
            {
                MongoDB.Bson.BsonDocument bd = BH.Engine.Serialiser.Convert.ToBson(db); //Break the reference clone issue

                BH.oM.XML.Environment.DocumentBuilder dbBroken = (BH.oM.XML.Environment.DocumentBuilder)BH.Engine.Serialiser.Convert.FromBson(bd);
                SerializeLevels(dbBroken.Levels, dbBroken.ElementsAsSpaces, gbx, exportType);
                SerializeCollection(dbBroken.ElementsAsSpaces, dbBroken.Levels, dbBroken.UnassignedPanels, gbx, exportType);
                SerializeCollection(dbBroken.ShadingElements, gbx, exportType);
                //SerializeCollection(dbBroken.UnassignedPanels, gbx, exportType);
            }
        }

        private static void SerializeBuildingShell(IEnumerable<BH.oM.XML.Environment.DocumentBuilder> documents, GBXML gbx, ExportType exportType)
        {
            foreach (BH.oM.XML.Environment.DocumentBuilder db in documents)
            {
                SerializeLevels(db.Levels, db.ElementsAsSpaces, gbx, exportType);
                SerializeCollection(db.ElementsAsSpaces.ExternalElements(), db.Levels, db.UnassignedPanels, gbx, exportType);
                SerializeCollection(db.ShadingElements, gbx, exportType);
            }
        }

        private static void SerializeBuildingSpaces(IEnumerable<BH.oM.XML.Environment.DocumentBuilder> documents, ExportType exportType, string filePath, string projectName)
        {
            filePath = System.IO.Path.Combine(filePath, projectName);
            if (!System.IO.Directory.Exists(filePath))
                System.IO.Directory.CreateDirectory(filePath);

            foreach (BH.oM.XML.Environment.DocumentBuilder db in documents)
            {
                foreach(List<Panel> space in db.ElementsAsSpaces)
                {
                    if (space.IsExternal(db.ElementsAsSpaces))
                    {
                        Dictionary<String, object> spaceData = (space.Where(x => x.CustomData.ContainsKey("Space_Custom_Data")).FirstOrDefault() != null ? space.Where(x => x.CustomData.ContainsKey("Space_Custom_Data")).FirstOrDefault().CustomData["Space_Custom_Data"] as Dictionary<String, object> : new Dictionary<string, object>());
                        string spaceName = "";
                        if (spaceData.ContainsKey("SAM_SpaceName") && spaceData["SAM_SpaceName"] != null)
                            spaceName = spaceData["SAM_SpaceName"].ToString();
                        else
                            spaceName = "Space-" + Guid.NewGuid().ToString().Replace("-", "");

                        GBXML gbx = new GBXML();
                        SerializeCollection(space, db.Levels, db.UnassignedPanels, gbx, exportType);

                        //Document History
                        DocumentHistory DocumentHistory = new DocumentHistory();
                        DocumentHistory.CreatedBy.Date = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                        gbx.DocumentHistory = DocumentHistory;

                        XMLWriter.Save(filePath, spaceName, gbx);
                    }
                }
            }
        }
    }

}