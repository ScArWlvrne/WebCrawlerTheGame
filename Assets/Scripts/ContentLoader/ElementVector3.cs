using System;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

[Serializable]
public class ElementVector3
{
    public float x;
    public float y;
    public float z;

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}

public class ElementVector3YamlConverter : IYamlTypeConverter
{
    public bool Accepts(Type type)
    {
        return type == typeof(ElementVector3);
    }   

    public object ReadYaml(IParser parser, Type type)
    {
        var vector3 = new ElementVector3();

        parser.Consume<SequenceStart>();

        int fieldCount = 0;
        while (!parser.TryConsume<SequenceEnd>(out _))
        {
            var scalar = parser.Consume<Scalar>().Value;

            switch (fieldCount)
            {
                case 0:
                    vector3.x = float.Parse(scalar, System.Globalization.CultureInfo.InvariantCulture);
                    fieldCount++;
                    break;
                case 1:
                    vector3.y = float.Parse(scalar, System.Globalization.CultureInfo.InvariantCulture);
                    fieldCount++;
                    break;
                case 2:
                    vector3.z = float.Parse(scalar, System.Globalization.CultureInfo.InvariantCulture);
                    fieldCount++;
                    break;
                default:
                    throw new InvalidOperationException("Too many fields in Vector3 YAML.");
            }
        }

        if (fieldCount != 3)
        {
            throw new InvalidOperationException("Not enough fields in Vector3 YAML.");
        }

        return vector3;
    }

    public void WriteYaml(IEmitter emitter, object value, Type type)
    {
        var vector3 = (ElementVector3)value;

        emitter.Emit(new SequenceStart(null, null, false, SequenceStyle.Flow));
        emitter.Emit(new Scalar(vector3.x.ToString(System.Globalization.CultureInfo.InvariantCulture)));
        emitter.Emit(new Scalar(vector3.y.ToString(System.Globalization.CultureInfo.InvariantCulture)));
        emitter.Emit(new Scalar(vector3.z.ToString(System.Globalization.CultureInfo.InvariantCulture)));
        emitter.Emit(new SequenceEnd());
    }
}