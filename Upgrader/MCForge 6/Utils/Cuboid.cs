/*
Copyright 2012 MCForge
Dual-licensed under the Educational Community License, Version 2.0 and
the GNU General Public License, Version 3 (the "Licenses"); you may
not use this file except in compliance with the Licenses. You may
obtain a copy of the Licenses at
http://www.opensource.org/licenses/ecl2.php
http://www.gnu.org/licenses/gpl-3.0.html
Unless required by applicable law or agreed to in writing,
software distributed under the Licenses are distributed on an "AS IS"
BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
or implied. See the Licenses for the specific language governing
permissions and limitations under the Licenses.
*/
using System;
using System.Collections.Generic;
using System.Globalization;
using MCForge.World;

namespace MCForge.Utils
{
    /// <summary>
    /// A basic class for Cuboiding
    /// </summary>
    /// <remarks></remarks>
    public class Cuboid
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Cuboid"/> class.
        /// </summary>
        /// <remarks>Size is 0</remarks>
        public Cuboid()
        {
            Max = Min = new Vector3D(0, 0, 0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cuboid"/> class.
        /// </summary>
        /// <param name="point1">The first <see cref="Vector3D"/>.</param>
        /// <param name="point2">The second <see cref="Vector3D"/>.</param>
        /// <remarks></remarks>
        public Cuboid(Vector3D point1, Vector3D point2)
        {
            Max = new Vector3D(
                Math.Max(point1.x, point2.x),
                Math.Max(point1.z, point2.z),
                Math.Max(point1.y, point2.y)
                );

            Min = new Vector3D(
                Math.Min(point1.x, point2.x),
                Math.Min(point1.z, point2.z),
                Math.Min(point1.y, point2.y)
                );
        }

        /// <summary>
        /// Gets or sets the maximum position.
        /// </summary>
        /// <value>The max position.</value>
        /// <remarks></remarks>
        public Vector3D Max { get; set; }

        /// <summary>
        /// Gets or sets the minimum position.
        /// </summary>
        /// <value>The minimum position.</value>
        /// <remarks></remarks>
        public Vector3D Min { get; set; }

        public bool Within(Vector3D pos)
        {
            return Max.x >= pos.x &&
                   Max.z >= pos.z && 
                   Max.y >= pos.y && 
                   Min.x <= pos.x && 
                   Min.z <= pos.z && 
                   Min.y <= pos.y;
        }

        #region for the cuboid

        private static void BufferAdd(ICollection<Pos> list, ushort x, ushort z, ushort y)
        {
            BufferAdd(list, new Vector3S(x, z, y));
        }

        private static void BufferAdd(ICollection<Pos> list, Vector3S type)
        {
            Pos pos;
            pos.pos = type;
            list.Add(pos);
        }

        private struct Pos
        {
            public Vector3S pos;
        }

        #endregion
    }
}