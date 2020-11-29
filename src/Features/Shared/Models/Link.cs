namespace CrewScheduling.Api.Features.Shared.Models
{
    /// <summary> Hypermedia information about the resource. </summary>
    public class Link
    {
        /// <summary> The universal location for the resource. </summary>
        /// <example> /v1/pilots/1 </example>
        public string Href { get; set; }

        /// <summary> The elationship to the resource. </summary>
        /// <example> self </example>
        public string Rel { get; set; }

        /// <summary> The HTTP verb to invoke on resource with. </summary>
        /// <example> GET </example>
        public string Method { get; set; }
    }
}
