rem call: Rip.bat inputdriveletter outputfolder discname

mkdir %2
"C:\Program Files (x86)\Handbrake\HandBrakeCLI" -f mkv -m -e x264 -q 20 --vfr -E faac --ab 160 --strict-anamorphic --decomb --subtitle scan --subtitle-forced --subtitle-burn --native-language eng --mixdown dp12 --min-duration 3600 --main-feature -v -o "%2\%3.mkv" -i %1
