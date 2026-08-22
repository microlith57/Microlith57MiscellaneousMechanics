local self = mu.trigger {
  "CameraDetector",
  name = "Camera Detector",
  desc = "Sets a flag when the camera can see the region."
}

self.flag "flag"
  :nonempty()
  :desc("Flag to set when the detector is onscreen.")

self.leniency(0.0)
  :desc("Extra margin around the camera in which it is still considered to see the detector.")

local function triggerText(_, trigger)
  return table.concat {"Camera Detector (", trigger.flag, ")"}
end

local res = self {
  category = "camera",
  triggerText = triggerText,
}

return res
