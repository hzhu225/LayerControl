// Written by Felix Zhu @ CMS Surveyors

/********************************************************************************************************************************************
     In this tutorial, you will learn how to control Layer properties in Civil3D.
     You don't need to write any code in this tutorial.
*********************************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;

namespace Tutorial_5
{
    public class Commands
    {
        [CommandMethod("Tut5")]
        public void Tutorial_5_Layer_control()
        {
            CivilDocument doc = CivilApplication.ActiveDocument;
            Document MdiActdoc = Application.DocumentManager.MdiActiveDocument;
            Editor editor = MdiActdoc.Editor;
            Database currentDB = MdiActdoc.Database;

            using (Transaction trans = HostApplicationServices.WorkingDatabase.TransactionManager.StartOpenCloseTransaction())
            {
                BlockTable blockTab = trans.GetObject(currentDB.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord modelSpaceBTR = trans.GetObject(blockTab[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                //We will still use same code as in Tutorial 3. You might noticed that the line is always created at current layer when you use Tut3 command.
                //Can we change the layer it goes? Of course.
                //If you type in myline and a dot (myline.), you will see there is a LayerId parameter. We can change the layer of Line by changing its layerId.
                //But it's asking for a layerId while what you only know is the layer name. How to get the corresponding layerId from a name?
                //Let's try to find layer "AAA"

         

                LayerTable layerTab = trans.GetObject(currentDB.LayerTableId, OpenMode.ForWrite) as LayerTable;       //Each layer is an instance of LayerTableRecord
                                                                                                                     //all LayerTableRecord in current drawing are stored in LayerTable


                ObjectId AAA_layerId = ObjectId.Null;                       //Declare a null instance of ObjectId to store the layerId we need.


                if (layerTab.Has("AAA") == true)                            //try to find if layer "AAA" exist already. "AAA" here is called a key, Table works like a dictionary.
                {
                    AAA_layerId = layerTab["AAA"];                          //layerTab["AAA"] returns the ObjectId of layer AAA
                }

                else                                                        //"AAA" doesn't exist, we need to create "AAA".
                {
                    LayerTableRecord myLayer = new LayerTableRecord();       //Create a new instance of LayerTableRecord .  "myLayer" is not Layer Name
                    myLayer.Name = "AAA";                                    //this is Layer Name. Type myLayer. to access more properties of layer.

                    layerTab.Add(myLayer);                                   //Add the new layer to layerTable
                    trans.AddNewlyCreatedDBObject(myLayer, true);            //Add the new layer to transaction. No need to add to modelSpace.

                    AAA_layerId = myLayer.ObjectId;
                }

                //************* Copy section above everytime you want to do something with layer. ************


                Point3d startPt = new Point3d(0, 0, 0);
                Point3d endPt = new Point3d(5, 5, 5);

                Line myline = new Line(startPt, endPt);
                myline.LayerId = AAA_layerId;                                   //now you can set the layerId

                modelSpaceBTR.AppendEntity(myline);
                trans.AddNewlyCreatedDBObject(myline, true);

                trans.Commit();
            }
        }
    }
}


//Build, load and use command "Tut5" several times in Civil3D.
//It will create a line at same position each time you run "Tut5". But it will only create "AAA" layer at first time.