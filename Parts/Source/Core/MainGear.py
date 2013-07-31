Units.Current = UnitTypes.Millimeters

PressureAngle = 20
Thickness = 3

# input = 44mm diameter, 1.25 diametral pitch
Diameter = 44
DiametralPitch = 1.25

MainGear = Part("MainGear")

Main = MainGear.AddGearDP("Profile", DiametralPitch, Diameter, PressureAngle, 0, 0, MainGear.GetPlane("XY-Plane"))
MainGear.AddExtrudeBoss("Gear", Main, Thickness, False)

NumberofTeeth = Main.NumberofTeeth

print "Number of teeth = %f\n" % NumberofTeeth
