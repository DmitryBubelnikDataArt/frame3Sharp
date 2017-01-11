﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using g3;

namespace f3
{
    public class XMLInputStream : IInputStream
    {
        public XmlDocument xml { get; set; }

        public event InputStream_AttributeHandler OnAttribute;
        public event InputStream_NodeHandler OnBeginScene;
        public event InputStream_NodeHandler OnBeginSceneObject;
        public event InputStream_NodeHandler OnEndScene;
        public event InputStream_NodeHandler OnEndSceneObject;
        public event InputStream_StructHandler OnBeginStruct;
        public event InputStream_StructHandler OnEndStruct;

        public void Restore()
        {
            XmlNodeList scenes = xml.SelectNodes("/" + IOStrings.Scene);
            foreach ( XmlNode scene in scenes ) {
                if ( OnBeginScene != null )
                    OnBeginScene();


                XmlNodeList sceneObjects = scene.SelectNodes(IOStrings.SceneObject);
                foreach ( XmlNode so in sceneObjects ) {
                    if (OnBeginSceneObject != null)
                        OnBeginSceneObject();

                    foreach ( XmlNode a in so.ChildNodes ) {
                        if (a.Name == IOStrings.Struct) 
                            restore_struct(a);
                        else
                            OnAttribute(a.Name, a.InnerText);
                    }

                    if (OnEndSceneObject != null)
                        OnEndSceneObject();
                }


                if (OnEndScene != null)
                    OnEndScene();
            }
        }


        void restore_struct(XmlNode structNode)
        {
            string sType = structNode.Attributes[IOStrings.StructType].InnerText;
            XmlNode identNode = structNode.Attributes.GetNamedItem(IOStrings.StructIdentifier);
            string sIdentifier = (identNode != null) ? identNode.InnerText : "";
            OnBeginStruct(sType, sIdentifier);
            foreach ( XmlNode a in structNode.ChildNodes ) {
                if (a.Name == IOStrings.Struct)
                    restore_struct(a);
                else
                    OnAttribute(a.Name, a.InnerText);
            }
            OnEndStruct(sType, sIdentifier);
        }
    }
}