namespace Prometheus;

/// <summary>
/// For managed lifetime metrics, we just want to uniquely identify metric instances so we can cache them.
/// We differentiate by the family name + the set of unique instance label names applied to the instance.
///
/// Managed lifetime metrics are not differentiated by static labels because the static labels are applied
/// in a lower layer (the underlying MetricFactory) and cannot differ within a single ManagedLifetimeMetricFactory.
/// </summary>
internal readonly struct ManagedLifetimeMetricIdentity(
    string metricFamilyName,
    StringSequence instanceLabelNames
) : IEquatable<ManagedLifetimeMetricIdentity>
{
    public readonly string MetricFamilyName = metricFamilyName;
    public readonly StringSequence InstanceLabelNames = instanceLabelNames;

    private readonly int _hashCode = CalculateHashCode(metricFamilyName, instanceLabelNames);

    public bool Equals(ManagedLifetimeMetricIdentity other)
    {
        return _hashCode == other._hashCode
            && string.Equals(MetricFamilyName, other.MetricFamilyName, StringComparison.Ordinal)
            && InstanceLabelNames.Equals(other.InstanceLabelNames);
    }

    public override int GetHashCode()
    {
        return _hashCode;
    }

    private static int CalculateHashCode(string metricFamilyName, StringSequence instanceLabelNames)
    {
        unchecked
        {
            var hashCode = 0;

            hashCode ^= metricFamilyName.GetHashCode() * 997;
            hashCode ^= instanceLabelNames.GetHashCode() * 397;

            return hashCode;
        }
    }

    public override string ToString()
    {
        return $"{MetricFamilyName}{InstanceLabelNames}";
    }

    public override bool Equals(object? obj)
    {
        return obj is ManagedLifetimeMetricIdentity identity && Equals(identity);
    }
}
