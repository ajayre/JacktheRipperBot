rem call: Rip.bat inputdriveletter outputfolder discname
rem eg: C:\Users\Me\Desktop\Rip.bat {Drive} C:\rips\{Timestamp} {Name}

mkdir %2
"C:\Program Files (x86)\Handbrake\HandBrakeCLI" --min-duration 3600 --main-feature --angle 1 -o "%2\%3.mkv"  -f mkv  --decomb -w 720 --loose-anamorphic  --modulus 2 -e x264 -q 20 --vfr -a 1 -E faac -6 dpl2 -R Auto -B 160 -D 0 --gain 0 --audio-fallback ffac3 --subtitle scan --subtitle-forced=1 --subtitle-burned=1 --native-language eng --x264-preset=veryfast  --x264-profile=main  --h264-level="4.0"  --verbose=1 -i %1
