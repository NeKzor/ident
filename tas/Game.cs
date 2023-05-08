/*
 * Copyright (c) 2023, NeKz
 *
 * SPDX-License-Identifier: MIT
 */

namespace Ident.TAS;

// ID of each map hotspot location.
public enum MapHotspotLocationId : int
{
    Lobby = 0,
    AdminOffice = 1,
    Library = 2,
    LandingPad = 3,
    Vault = 4,
}

// Game scenes.
public static class Scene
{
    public const string Intro = "titlesequence";
    public const string Map = "locMap";
    public const string Credits = "locCredits";
    public const string NoScene = "";
}
