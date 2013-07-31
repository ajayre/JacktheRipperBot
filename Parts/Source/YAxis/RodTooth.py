Units.Current = UnitTypes.Millimeters

PressureAngle = 20

InputTeeth = 120
DiametralPitch = 1.25

RodTooth = Part("RodTooth")

Profile = RodTooth.AddGearDN("Profile", DiametralPitch, InputTeeth, PressureAngle, 0, 0, RodTooth.GetPlane("XY-Plane"))

