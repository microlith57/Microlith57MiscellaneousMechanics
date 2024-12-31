# microlith57's miscellaneous mechanics

<p>
  Work-in-progress helper, released early with a few requested entities.
</p>
<p>
  Entities:
  <ul>
    <li><b>Rainbow Light:</b> light source that changes colour like rainbow spinners</li>
  </ul>
</p>
<p>
  Controller entities:
  <ul>
    <li><b>Color Packer & Unpacker:</b> convert between a compact color representation and its RGBA/HSL components</li>
    <li><b>Consumable Resource:</b> interact with the player's stamina / max stamina, or a completely custom slider-driven resource</li>
    <li><b>Focus:</b> press a button to fade a slider and perhaps consume a resource</li>
    <li><b>Freeze Time Active:</b> pause <code>Scene.TimeActive</code>, for stunning shenanigans</li>
    <li><b>Holdable Priority:</b> make it so you always pick up the "correct" holdable if you are in range of several (i.e. the one closest to your hand position; configurable)</li>
    <li><b>Lock Pause:</b> prevent the player from retrying, or save+quitting, or pausing, or a combination of these</li>
    <li><b>Player Facing Flag:</b> detect the player's facing direction</li>
    <li><b>Player Grounded Flag:</b> detect if the player is grounded</li>
    <li><b>Player State Name to Counter:</b> convert a state name (potentially for a custom state) into its ID</li>
    <li><b>Slider Controllers:</b> control ambience, music, camera zoom, colourgrades, sounds, stylegrounds, and the time rate via sliders</li>
  </ul>
</p>
<p>
  Triggers and regions:
  <ul>
    <li><b>Holdable Bouyancy Region:</b> makes holdables (and optionally the player) float, like it's a liquid</li>
    <li><b>Position Tracker Region:</b> detects the position/size of a chosen entity</li>
    <li><b>Set Facing Trigger:</b> sets the player's facing direction</li>
    <li><b>Slider Camera Triggers:</b> offset/target triggers whose properties are slider-driven</li>
    <li><b>Slider Trigger:</b> set a slider based on where the player is in the trigger</li>
  </ul>
</p>
<p>
  More mechanics are planned; if you need a mechanic, feel free to request one.
</p>

In addition, this helper contains some nonpublic stuff.
All code here is released under the MIT license, with the informal proviso that I would prefer if you ask me before using or copying code from the unreleased entities, or discussing them publicly without spoiler tags.

Some code from [Maddie's Helping Hand](https://github.com/maddie480/MaddieHelpingHand/), [Communal Helper](https://github.com/CommunalHelper/CommunalHelper/), [GravityHelper](https://github.com/swoolcock/GravityHelper/) has been used; see their licenses.
