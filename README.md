# Bullet-Proof-Youtube-Playback-for-WP7

# Project Description

## Why and how?

What we are doing is setting up an invisible browser control which can extract the mp4 cache url by interpretting javascript in YoutubeMp4Extractor.html.  Assuming the extractor is hosted and obtained remotely, we can repair In-App Youtube video playback functionality through changes on the server side by updating YoutubeMP4Extractor.html should the means to parse a youtube mp4 cache url change!  Thus saving us time and frustration to recertify an app in the marketplace!

This happens often enough that it's basically a timebomb to bake a youtube parser into a mobile application.  I created this to give peace of mind that WP7 apps could be bulletproofed against changes initiated by Youtube.

Examples of youtube breaking parsers include:

http://trac.videolan.org/vlc/ticket/4608

http://answers.yahoo.com/question/index?qid=20100403204443AAOk2EG

http://yazsoft.com/f/viewtopic.php?f=5&t=1230

## More how
For playback we are using the Microsoft Media Platform from: http://smf.codeplex.com

For whatever reason, I had to recompile SMF from source with shorter assembly names, therefore, this method of playback is not guaranteed with the official SMF Player build.  You can read more info on this @ http://smf.codeplex.com/discussions/280487
