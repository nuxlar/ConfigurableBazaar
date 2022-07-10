# ConfigurableBazaar
Server-side/Vanilla compatible. 
This mod aims to make the bazaar completely configurable by adding interactables, limiting lunar rolls/pickups, adding extra functionality, and preventing kickout for the party. Bazaar kickout prevention works for everyone even on vanilla clients. Also if only the host has the mod the green cauldron color will look red and scrap text won't show for vanilla players.
Currently has configuration for:

	- lunar prices
	- lunar rolls
	- lunar items per player
	- lunar items respawning after rolls
	- blue portal spawns
	- a scrapper
	- a cleansing pool
	- extra cauldrons (red/green to white)
	- printers (up to 4)
	- red cauldrons taking red scrap

## Config Info
**Portal Config:**
* 0 for no change (default), 12345 for a portal every stage
* the numbers apply to both pre and post loop
* the stage numbers that aren't in WONT spawn a portal even if you hit a newt shrine e.g. (135 wont spawn portals on stages 2 and 4)

## Contact
Reach out with bugs/feedback **nuxlar#0235**

## Donate
If you like what I'm doing contribute to my caffeine addiction! https://ko-fi.com/

## Changelog
**1.0.0**
* DELETE YOUR CONFIGURABLEBAZAAR CONFIG FILE IF YOURE UPDATING
* Adds config for bazaar prices (pods, seers, rolls)
* Adds config for using red scrap for red cauldrons
* Adds config for lunar items per player
* Adds config for rolling the lunar shop
* Adds config for lunar items respawning after each roll
* Adds broadcast message with rules/config
* Separates red to white and green to white cauldrons
* Fixes only host getting kickout exemption

**0.6.5**
* Removes bazaar kickout

**0.6.0**
* Added config for newt portal spawns
* Fixed the hologram text/color for realsies this time (i hope)

**0.5.4**
* Adds hologram text on cauldron that explicitly says the exchange rates

**0.5.3**
* Fixes hologram text on cauldron

**0.5.2**
* Fixes cauldron not spawning in multiplayer
* README edit.

**0.5.1**
* Readme edit.

**0.5.0**
* Adds config for cleansing pool and extra cauldron (red/green to white).

**0.3.0**
* Adds config for scrapper and printers (up to 4).
