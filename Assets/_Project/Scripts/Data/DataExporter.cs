using System.IO;
using System.Globalization;
using UnityEngine;
using BallisticSim.Core.Model;

namespace BallisticSim.Core.Data
{
    public static class DataExporter
    {
        private const string FILE_NAME = "ballistic_data.csv";

        public static void ExportToCsv(BallisticParameters parameters, BallisticResults results)
        {
            string path = Path.Combine(Application.persistentDataPath, FILE_NAME);
            bool fileExists = File.Exists(path);

            using (StreamWriter writer = new StreamWriter(path, true))
            {
                if (!fileExists)
                {
                    writer.WriteLine(
                        "Angle,Force,Mass,BulletSize,TargetDistance," +
                        "Distance,FlightTime,ImpactX,ImpactY,ImpactZ," +
                        "RelativeVelocity,CollisionImpulse,BrokenJoints"
                    );
                }

                writer.WriteLine(string.Format(
                    CultureInfo.InvariantCulture,
                    "{0:F2},{1:F2},{2:F2},{3:F2},{4:F2},{5:F2},{6:F2},{7:F2},{8:F2},{9:F2},{10:F2},{11:F2},{12}",
                    parameters.angle,
                    parameters.force,
                    parameters.mass,
                    parameters.bulletSize,
                    parameters.targetDistance,
                    results.distance,
                    results.flightTime,
                    results.impactPoint.x,
                    results.impactPoint.y,
                    results.impactPoint.z,
                    results.relativeVelocity,
                    results.collisionImpulse,
                    results.brokenJoints
                ));
            }

#if UNITY_EDITOR
            Debug.Log($"[{nameof(DataExporter)}] CSV exported to: {path}");
#endif
        }
    }
}
