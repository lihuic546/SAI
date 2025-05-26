using AUTD3Sharp;
using AUTD3Sharp.Utils;
using AUTD3Sharp.Link;
using AUTD3Sharp.Gain;
using AUTD3Sharp.Modulation;
using static AUTD3Sharp.Units;

using var autd = Controller.Open(
    [new AUTD3(pos: Point3.Origin, rot: Quaternion.Identity)],
    new SOEM(
        (slave, status) =>
            {
                Console.Error.WriteLine($"slave [{slave}]: {status}");
                if (status == Status.Lost)
                    Environment.Exit(-1);
            },
        new SOEMOption()
    )
);

var firmList = autd.FirmwareVersion();
foreach (var firm in firmList)
    Console.WriteLine(firm);

autd.Send(new Silencer());

var g = new Focus(
    pos: autd.Center() + new Vector3(0, 0, 150),
    option: new FocusOption()
);
var m = new Sine(
    freq: 150u * Hz,
    option: new SineOption()
);
autd.Send((m, g));

Console.ReadKey(true);

autd.Close();
