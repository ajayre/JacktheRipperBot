Units.Current = UnitTypes.Millimeters

PressureAngle = 20
Thickness = 3

# input = 15 teeth, 12mm diameter
InputTeeth = 20
InputDiameter = 16

ServoGear = Part("Servo Gear")

Input = ServoGear.AddGearNP("Profile", InputTeeth, InputDiameter, PressureAngle, 0, 0, ServoGear.GetPlane("XY-Plane"))
ServoGear.AddExtrudeBoss("Gear", Input, Thickness, False)

DiametralPitch = Input.DiametralPitch

print "Diametral Pitch = %f\n" % DiametralPitch
