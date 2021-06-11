# coding: utf-8
import sys

if __name__ == "__main__":
    from toascii import VideoConverter, gradients
    v = VideoConverter(sys.argv[1], scale=float(sys.argv[2]), width_stretch=float(sys.argv[3]), gradient=sys.argv[4])
    v.convert()

    a = 0
    b = len(v.ascii_frames)

    output = ""
    while a < b:
        frms = v.ascii_frames[a].split("\n")
        for x in range(0, len(frms), 2):
            if x == 0:
                output += "   BA_" + str(a).zfill(4) + ".bmp"
            output += frms[x] + "\n"
        output += "\t"
        a +=1 
        
    with open("output.txt", "w", encoding="utf-8") as text_file:
        text_file.write(output)