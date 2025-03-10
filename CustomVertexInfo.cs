﻿using Microsoft.Xna.Framework.Graphics;

namespace WeaponDisplay
{
    public struct CustomVertexInfo : IVertexType
    {
        private static VertexDeclaration _vertexDeclaration = new VertexDeclaration(new VertexElement[3]
        {
                new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
                new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0)
        });
        public Vector2 Position;
        public Color Color;
        public Vector3 TexCoord;

        public CustomVertexInfo(Vector2 position, Color color, Vector3 texCoord) {
            this.Position = position;
            this.Color = color;
            this.TexCoord = texCoord;
        }

        public CustomVertexInfo(Vector2 position, Vector3 texCoord) {
            this.Position = position;
            this.Color = Color.White;
            this.TexCoord = texCoord;
        }

        public VertexDeclaration VertexDeclaration {
            get {
                return _vertexDeclaration;
            }
        }
    }
}
