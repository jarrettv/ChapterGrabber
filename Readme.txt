ChapterGrabber

What is it?
--------------------
ChapterGrabber is made to extract chapter times from DVD and
BluRay discs and combine them with chapter names from the 
internet.  It produces chapter text files that are useful when
muxing matroska or mp4 files.

Install
--------------------
You must have .NET Framework 3.5 available from Windows Update.

Whats New?
--------------------
2013-02-09 : v5.4
Bug fix for opening chapter files

2013-02-08 : v5.3
Force usage of new database API key
Added support for delaying all chapter times (time shift)
Improved search results

2011-12-22 : v5.2
Small bugfixes to database and searches
Added link to new database website

! missing changes !

2010-06-04 : v4.4
Queue search so interface stays responsive (add wait indicator)
Add new support for choosing correct title with help of Google
Title indictor for when good title chosen

2010-06-04 : v4.4
Queue search so interface stays responsive (add wait indicator)
Add new support for choosing correct title with help of Google
Title indictor for when good title chosen

2010-06-04 : v4.3
Queue database work to keep from slowing UI down
Show animation while waiting on database work
Fix import from clipboard for chapters copied from spun website

2010-06-03 : v4.2
Added new online database support from chapterdb.org
Names automatically loaded when found in database
Database automatically updated upon save

2010-05-14 : v4.1
Added automatic application updater that downloads and installs new updates

2010-05-13 : v4.0
Added update-checking and auto notification
Added chapters file association

2010-05-10 : v3.9
Update TagChimpGrabber to specify type of movie

2010-05-09 : v3.8
Removed MetaGrabber
Fixed TagChimpGrabber

2009-05-01 : v3.7 
Attempt to fix globalization issues for non-US cultures
Source code included

2009-01-23 : v3.6
HD-DVD support added.  You can now extract chapters from the disc or directly from an XPL file.
4 new output formats added: TsMuxeR Meta, Timecodes, Celltimes, x264 QP File.
You can now change current FPS without recalculating chapter times.

2009-01-09 : v3.5
New source for chapter names can now be grabbed from metaservices.
ChapterGrabber now stores last open directory in settings and points to parent directory when it doesn't exist. 

2008-12-19 : v3.4
ChapterGrabber can now detect the fps of BluRay discs via the CLIPINFO data.  It does not yet support extracting the fps when directly opening the MPLS file.
The new ChapterGrabber format has been finalized and can be properly loaded and saved.
When moving the chapters up or down, only the names are moved and not the times.
I also added some additional framerates to the config file, 50fps and 60/1.001fps.

2008-12-06 : v3.3
Two new output formats added: Matroska XML and ChapterGrabber
XML formats (no support yet for loading these files).  You can
now choose a language to apply to all chapter names through a new
menu. ChapterGrabber now detects and removes invalid characters 
in the tagChimp search results.

2008-12-02 : v3.2
IFO parsing was re-written with increased accuracy.  It also no
longer depends on vStrip.dll for IFO parsing.  You can now
change the FPS of your chapters in case you do a pulldown or
want to switch chapter times from NTSC to PAL. A new menu for
recently opened files is now available.  Bluray FPS is not yet
detected.  A new configuration file stores user and app settings.

2008-11-25 : v3.1
New support for reading chapters directly from BluRay discs.
Also, IFO parsing was optimized and you can now read chapters
directly from DVD discs without having to choose the IFO file.
A new setting allows you to ignore the short last chapter that
sometimes occur at the very end.

2008-11-19 : v3.0
Updated to .NET Framework 3.5.  All changes prior to 2.0 were
lost. :( IFO parsing was rewritten based on Zulu's previous work.
I've added tagchimp chapter title import.  You can search for
your title and then choose from the search results.  I've 
disabled the import from web as amazon no longer has chapter
names on their website.

2004-03-25 : v2.3
Updated to Zulu's newest library for the IFO support.  Created
a new output format for creating Matroska XML chapter files.
The up and down arrows only move the chapter names now.

2004-01-30 : v2.2.1
This version adds a new framerate to the menu 23.999fps.  The
registry key is no longer hard coded and can be changed in the
config file. 

2003-12-07 : v2.2
Updated XviD 1.0 Zones file creation to create only one zone
for each chapter. It also sets the weight to 1 (100%). I added
a new option to choose your framerate. 24.000fps seemed to work
well in my test for NTSC DVD's that are inverse telecined. You
will need the newest build of XviD for zones to work correctly.

2003-12-06 : v2.1
Added XviD 1.0 Zones file creation.  It creates a registry file
that can be easily merged to modify XviD's settings to force 
keyframes at chapters.  I had to modify MAX_ZONES in config.h
to double the number of zones from 64 to a more reasonable 128.
Everything is handled as 23.976fps material.  Will fix later.
BTW, this feature needs testing.

2003-09-15 : v2.0
First release supporting direct ifo parsing thanks to zulu
Therefore chapterXtractor is no longer needed.

Directions
--------------------
1. Open your IFO or TXT file containing chapter times
2. Type in the title of your movie
3. Click search to download chapter names
4. Choose the best result
4. Save your new TXT file

Thanks to
--------------------
- BDInfo
- OpenMediaLibrary
- The tagChimp
  http://tagchimp.com/
- The ChapterStripper code by zulu (no longer in use)
  http://zuludev.de.vu
- The vStrip source code (in a modified DLL) by maven
  http://www.maven.de

Author
--------------------
For bug report, new feature use:
http://jvance.com/pages/ChapterGrabber.xhtml

Source Code
--------------------
http://code.google.com/p/chaptergrabber/
