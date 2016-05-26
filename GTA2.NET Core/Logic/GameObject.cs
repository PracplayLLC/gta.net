﻿using FarseerPhysics.Collision.Shapes;
// GTA2.NET
// 
// File: GameplayObject.cs
// Created: 14.07.2013
// 
// 
// Copyright (C) 2010-2013 Hiale
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software
// and associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software
// is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies
// or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Grand Theft Auto (GTA) is a registred trademark of Rockstar Games.
using Hiale.GTA2NET.Core.Helper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Hiale.GTA2NET.Core.Logic
{
    /// <summary>
    /// Represent a Game Object.
    /// </summary>
    public abstract class GameObject
    {
        /// <summary>
        /// Dictionary that contains all the sprites.
        /// </summary>
        public static Sprites spriteAtlas;

        private Vector3 postition;
        /// <summary>
        /// Current position of this object. It represents the center of the object.
        /// </summary>
        public Vector3 Position3
        {
            get { return postition; }
            protected set 
            {
                postition = value;
                drawCoordinates.CenterPos = postition;
            }
        }

        /// <summary>
        /// 2D position of the object
        /// </summary>
        public Vector2 Position2
        {
            get
            {
                return new Vector2(Position3.X, Position3.Y);
            }
        }

        private float rotationAngle;
        /// <summary>
        /// Current rotation angle in radians between 0 and 2pi.
        /// </summary>
        public float RotationAngle
        {
            get { return rotationAngle; }
            set
            {
                if (value < 0)
                    value = MathHelper.TwoPi + value;
                else if (value > MathHelper.TwoPi)
                    value = value - MathHelper.TwoPi;
                rotationAngle = value;
                spriteRotation = rotationAngle + MathHelper.PiOver2;
            }
        }

        /// <summary>
        /// Store the Sprite ID for this object
        /// </summary>
        protected uint spriteID;        

        /// <summary>
        /// The rotation of the sprite.
        /// </summary>
        /// <remarks>Since the angles in GTA2 are weird, so this values are always -pi/2 or -90º defased.</remarks>
        protected float spriteRotation { get; private set; }

        /// <summary>
        /// A list with all vertices's used to draw this object.
        /// </summary>
        protected List<VertexPositionNormalTexture> verticesCollection;

        /// <summary>
        /// List with the way the vertices's are used.
        /// </summary>
        protected List<int> indicesCollection;

        /// <summary>
        /// The shape of the object.
        /// </summary>
        protected CompactRectangle shape;  //ToDo: its may be a good idea to used something other than a rectangle...

        protected FaceCoordinates drawCoordinates;
        /// <summary>
        /// Creates all the necessary things to draw the object
        /// </summary>
        /// <returns>A ModelData with the data to draw the object</returns>
        public virtual Frame Draw() 
        {
            verticesCollection.Clear();
            indicesCollection.Clear();

            FaceCoordinates rotetedCoords = drawCoordinates.Rotate(RotationAngle);
            // calculate the coordinates of the for vertices.
            Vector3 tLeft = rotetedCoords.TopLeft;
            Vector3 tRight = rotetedCoords.TopRight;
            Vector3 bLeft = rotetedCoords.BottomLeft;
            Vector3 bRight = rotetedCoords.BottomRight;

            Vector2[] texture = spriteAtlas.GetSprite(spriteID);              

            verticesCollection.Add(new VertexPositionNormalTexture(tRight, Vector3.Zero, texture[1]));
            verticesCollection.Add(new VertexPositionNormalTexture(bRight, Vector3.Zero, texture[0]));
            verticesCollection.Add(new VertexPositionNormalTexture(tLeft, Vector3.Zero, texture[2]));
            verticesCollection.Add(new VertexPositionNormalTexture(bLeft, Vector3.Zero, texture[3]));

            int startIndex = verticesCollection.Count - 4;
            indicesCollection.Add(startIndex);
            indicesCollection.Add(startIndex + 1);
            indicesCollection.Add(startIndex + 2);
            indicesCollection.Add(startIndex + 1);
            indicesCollection.Add(startIndex + 3);
            indicesCollection.Add(startIndex + 2);

            return new Frame(null, null, verticesCollection, indicesCollection, new Vector2());
        }

        /// <summary>
        /// Updates the state of the Object
        /// </summary>
        /// <param name="elapsedTime">The time occurred since the last Update</param>
        public abstract void Update(float elapsedTime);
        
        /// <summary>
        /// Creates a instance of GameObject
        /// </summary>
        /// <param name="startUpPosition">The initial position for the object</param>
        /// <param name="startUpRotation">The initial rotation of the object</param>
        /// <param name="shape">The shape of the object</param>
        protected GameObject(Vector3 startUpPosition, float startUpRotation, CompactRectangle shape)
        {
            drawCoordinates = new FaceCoordinates(startUpPosition, shape.Width, shape.Height);
            Position3 = startUpPosition;
            RotationAngle = startUpRotation;
            this.shape = shape;
            this.spriteID = 0;
            verticesCollection = new List<VertexPositionNormalTexture>();
            indicesCollection = new List<int>();
        }
    }
}
