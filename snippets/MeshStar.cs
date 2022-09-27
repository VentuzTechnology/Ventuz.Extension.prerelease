    [VxDefaultValue(5, "JagCount")]
    [VxDefaultValue(1.0f, "Size")]
    [VxDefaultValue(0.6f, "Inner")]
    [VxNumeric(0.0f, float.MaxValue, 0.1f, "Size")]
    [VxNumeric(0.0f, 1.0f, 0.01f, "Inner")]
    [VxNumeric(2, 200, 1, "JagCount")]
    [VxDynamicIcon("Geo")]
    public class MeshStarVx : VxContentNode
    {
        public struct PS : IMeshResourceParameter
        {
            public int JagCount;
            public float Size;
            public float Inner;

            public IMesh GenerateMesh(IMeshFactory meshFactory)
            {
                // capture
                var _JagCount = JagCount;
                var _Size = Size;
                var _Inner = Inner;

                // coerse
                if ( _JagCount < 2 ) _JagCount = 2;
                if ( _Size < 0.0f ) _Size = 0.0f;
                if ( _Inner < 0.0f ) _Inner = 0.0f;

                var nVertices = 1 + _JagCount * 2; // centerpoint plus jags (inner and outer point)
                var nIndicies = _JagCount * 2 * 3;

                return meshFactory.CreateTriangleList<VertexPN, ushort>(nVertices, nIndicies, (_v, _i) =>
                {
                    unsafe
                    {
                        VertexPN* v = (VertexPN*)_v.ToPointer();
                        ushort* i = (ushort*)_i.ToPointer();


                        // last vertex is the center
                        var center = (ushort)(nVertices - 1);
                        v[center].nz = -1.0f;

                        var jags = (ushort)(_JagCount * 2);
                        var lastJag = (ushort)(jags - 1);
                        var a = 360.0 / jags; // with 4 jags a is an 8th of a circle
                        int f = 0;
                        for ( ushort j = 0; j < jags; j++ )
                        {
                            var s = _Size;

                            // odd items are inner
                            if ( (j & 1) != 0 )
                                s *= _Inner;

                            var aa = a * j;
                            v[j].px = (float)Math.Sin(aa / 180.0 * Math.PI) * s;
                            v[j].py = (float)Math.Cos(aa / 180.0 * Math.PI) * s;
                            v[j].nz = -1.0f;

                            i[f++] = center;
                            i[f++] = j;
                            i[f++] = j == lastJag ? (ushort)0 : (ushort)(j + 1);
                        }
                    }
                });
            }
        }

        public IMesh ValidateGeo(int JagCount, float Size, float Inner)
            => CreateMesh("Geo", new PS() { JagCount = JagCount, Size = Size, Inner = Inner });
    }
