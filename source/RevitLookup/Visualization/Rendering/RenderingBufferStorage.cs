using Autodesk.Revit.DB.DirectContext3D;

namespace RevitLookup.Visualization.Rendering;

/// <summary>
///     Represents the vertex, index, and effect buffers of a single visualization render pass.
/// </summary>
public sealed class RenderingBufferStorage : IDisposable
{
    /// <summary>
    ///     Gets or sets the vertex format flags of the buffer.
    /// </summary>
    public VertexFormatBits FormatBits { get; set; }

    /// <summary>
    ///     Gets or sets the number of primitives in the buffer.
    /// </summary>
    public int PrimitiveCount { get; set; }

    /// <summary>
    ///     Gets or sets the number of vertices in the buffer.
    /// </summary>
    public int VertexBufferCount { get; set; }

    /// <summary>
    ///     Gets or sets the number of indices in the buffer.
    /// </summary>
    public int IndexBufferCount { get; set; }

    /// <summary>
    ///     Gets or sets the vertex buffer.
    /// </summary>
    public VertexBuffer? VertexBuffer { get; set; }

    /// <summary>
    ///     Gets or sets the index buffer.
    /// </summary>
    public IndexBuffer? IndexBuffer { get; set; }

    /// <summary>
    ///     Gets or sets the vertex format built from <see cref="FormatBits" />.
    /// </summary>
    public VertexFormat? VertexFormat { get; set; }

    /// <summary>
    ///     Gets or sets the effect the buffer is rendered with.
    /// </summary>
    public EffectInstance? EffectInstance { get; set; }

    /// <inheritdoc />
    /// <remarks>Also disposes the effect instance, in addition to the buffers <see cref="DisposeBuffers" /> disposes.</remarks>
    public void Dispose()
    {
        DisposeBuffers();
        EffectInstance?.Dispose();
        EffectInstance = null;
    }

    /// <summary>
    ///     Returns a value indicating whether the vertex buffer, index buffer, vertex format, and effect are all valid.
    /// </summary>
    /// <returns><see langword="true" /> if all buffers are valid; otherwise, <see langword="false" />.</returns>
    public bool IsValid()
    {
        if (VertexBuffer is null || !VertexBuffer.IsValid())
        {
            return false;
        }

        if (IndexBuffer is null || !IndexBuffer.IsValid())
        {
            return false;
        }

        if (VertexFormat is null || !VertexFormat.IsValid())
        {
            return false;
        }

        if (EffectInstance is null || !EffectInstance.IsValid())
        {
            return false;
        }

        return true;
    }

    /// <summary>
    ///     Disposes the vertex buffer, index buffer, and vertex format.
    /// </summary>
    /// <remarks>The effect instance is left untouched.</remarks>
    public void DisposeBuffers()
    {
        VertexBuffer?.Dispose();
        VertexBuffer = null;
        IndexBuffer?.Dispose();
        IndexBuffer = null;
        VertexFormat?.Dispose();
        VertexFormat = null;
    }
}
