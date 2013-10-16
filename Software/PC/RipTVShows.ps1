$drive = $args[0]
$folder = $args[1]
$name = $args[2]

for ($title=1; $title -le 40; $title++) {
  $mkvfile = $("$folder\$name" + "_$title.mkv")
  &"C:\Program Files (x86)\Handbrake\HandBrakeCLI.exe" --min-duration 1200 --angle 1 -o "$mkvfile"  -f mkv  --decomb -w 720 --loose-anamorphic  --modulus 2 -e x264 -q 20 --vfr -a 1 -E faac -6 dpl2 -R Auto -B 160 -D 0 --gain 0 --audio-fallback ffac3 --subtitle scan --subtitle-forced=1 --subtitle-burned=1 --native-language eng --x264-preset=veryfast  --x264-profile=main  --h264-level="4.0"  --verbose=1 --input $drive --title $title
}
