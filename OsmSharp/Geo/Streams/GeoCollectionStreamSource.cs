﻿// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using OsmSharp.Geo.Geometries;
using OsmSharp.Geo.Geometries.Streams;
using OsmSharp.Math.Geo;

namespace OsmSharp.Geo.Streams
{
    /// <summary>
    /// A geometry stream source for a geometry collection.
    /// </summary>
    public class GeoCollectionStreamSource : IGeoStreamSource
    {
        /// <summary>
        /// Creates a new geometry collection stream source.
        /// </summary>
        /// <param name="collection"></param>
        public GeoCollectionStreamSource(GeometryCollection collection)
        {
            this.GeometryCollection = collection;
        }

        /// <summary>
        /// Gets/sets the geometry collection.
        /// </summary>
        public GeometryCollection GeometryCollection { get; private set; }

        /// <summary>
        /// Initializes this stream source.
        /// </summary>
        public virtual void Initialize()
        {
            _enumerator = this.GeometryCollection.GetEnumerator();
        }

        /// <summary>
        /// Returns true if this stream can be reset.
        /// </summary>
        /// <returns></returns>
        public virtual bool CanReset()
        {
            return true;
        }

        /// <summary>
        /// Closes this stream.
        /// </summary>
        public virtual void Close()
        {
            _enumerator = null;
        }

        /// <summary>
        /// Returns true if this stream has bounds.
        /// </summary>
        public bool HasBounds
        {
            get { return true; }
        }

        /// <summary>
        /// Returns the bounds of this geometry source.
        /// </summary>
        /// <returns></returns>
        public GeoCoordinateBox GetBounds()
        {
            return this.GeometryCollection.Box;
        }

        /// <summary>
        /// Returns the current geometry.
        /// </summary>
        public Geometry Current
        {
            get
            {
                if (_enumerator == null) throw new InvalidOperationException("Stream not initialized.");

                return _enumerator.Current;
            }
        }

        /// <summary>
        /// Returns the current geometry.
        /// </summary>
        object System.Collections.IEnumerator.Current
        {
            get { return this.Current; }
        }

        /// <summary>
        /// Disposes all resource associated with this source.
        /// </summary>
        public virtual void Dispose()
        {

        }

        /// <summary>
        /// Holds the current enumerator.
        /// </summary>
        private IEnumerator<Geometry> _enumerator;

        /// <summary>
        /// Move to the next item in the geometry collection.
        /// </summary>
        /// <returns></returns>
        public virtual bool MoveNext()
        {
            if (_enumerator == null) throw new InvalidOperationException("Stream not initialized.");

            return _enumerator.MoveNext();
        }

        /// <summary>
        /// Resets this 
        /// </summary>
        public virtual void Reset()
        {
            // remove all the stuff that's there.
            _enumerator = null;

            this.Initialize();
        }

        /// <summary>
        /// Returns a enumerator for this source.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Geometry> GetEnumerator()
        {
            this.Initialize();

            return this;
        }

        /// <summary>
        /// Returns an enumerator for the objects in this source.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}