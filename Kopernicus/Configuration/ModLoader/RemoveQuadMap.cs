﻿/**
 * Kopernicus Planetary System Modifier
 * Copyright (C) 2014 Bryce C Schroeder (bryce.schroeder@gmail.com), Nathaniel R. Lewis (linux.robotdude@gmail.com)
 * 
 * http://www.ferazelhosting.net/~bryce/contact.html
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston,
 * MA 02110-1301  USA
 * 
 * This library is intended to be used as a plugin for Kerbal Space Program
 * which is copyright 2011-2014 Squad. Your usage of Kerbal Space Program
 * itself is governed by the terms of its EULA, not the license above.
 * 
 * https://kerbalspaceprogram.com
 */

using System;
using UnityEngine;

namespace Kopernicus
{
	namespace Configuration
	{
		namespace ModLoader
		{
			[RequireConfigType(ConfigType.Node)]
            public class RemoveQuadMap : ModLoader, IParserEventSubscriber
			{
				// Actual PQS mod we are loading
				private PQSMod_RemoveQuadMap _mod;

                // If we have no PQS-Mods
                [ParserTarget("generateMap", optional = true)]
                private NumericParser<bool> generateMap
                {
                    set
                    {
                        if (value.value)
                        {
                            Texture2D tex = new Texture2D(1, 1);
                            tex.SetPixel(1, 1, Color.black);
                            tex.Apply();
                            _mod.map = new MapSO();
                            _mod.map.CreateMap(MapSO.MapDepth.Greyscale, tex);
                        }
                    }
                }

                // The map texture for the Quad Remover (?)
				[ParserTarget("map", optional = true)]
				private MapSOParser_GreyScale<MapSO> map
				{
					set { _mod.map = value.value; }
				}

                // The deformity of the map for the Quad Remover (?)
                [ParserTarget("deformity", optional = true)]
				private NumericParser<float> mapDeformity
				{
					set { _mod.mapDeformity = value.value; }
				}

                // The max. height for the Quad Remover (?)
                [ParserTarget("maxHeight", optional = true)]
				private NumericParser<float> maxHeight
				{
					set { _mod.maxHeight = value.value; }
				}

                // The min texture for the Quad Remover (?)
                [ParserTarget("minHeight", optional = true)]
				private NumericParser<float> minHeight 
				{
					set { _mod.minHeight = value.value; }
				}

				void IParserEventSubscriber.Apply(ConfigNode node)
				{
                   
				}

				void IParserEventSubscriber.PostApply(ConfigNode node)
				{

				}

                public RemoveQuadMap()
				{
					// Create the base mod
                    GameObject modObject = new GameObject("RemoveQuadMap");
					modObject.transform.parent = Utility.Deactivator;
                    _mod = modObject.AddComponent<PQSMod_RemoveQuadMap>();
					base.mod = _mod;
				}
			}
		}
	}
}

