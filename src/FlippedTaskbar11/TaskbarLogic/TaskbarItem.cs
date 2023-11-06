/*
 * Credits to https://gist.github.com/emoacht
 * https://gist.github.com/emoacht/bb6c0305e24a915e4e233005aad605d5
 */

namespace TopCenterStart11.TaskbarLogic;

internal class TaskbarItem
{
    public string DeviceInstanceId { get; }
    public IntPtr Handle { get; }
    public bool IsPrimary { get; }
    public TaskbarAlignment Alignment { get; }
    public WIN32.RECT TaskbarRect { get; }
    public WIN32.RECT MonitorRect { get; }
    public WIN32.RECT WorkRect { get; }

    public TaskbarItem(
        string deviceInstanceId,
        IntPtr handle,
        bool isPrimary,
        TaskbarAlignment alignment,
        WIN32.RECT taskbarRect,
        WIN32.RECT monitorRect,
        WIN32.RECT workRect)
    {
        this.DeviceInstanceId = deviceInstanceId;
        this.Handle = handle;
        this.IsPrimary = isPrimary;
        this.Alignment = alignment;
        this.TaskbarRect = taskbarRect;
        this.MonitorRect = monitorRect;
        this.WorkRect = workRect;
    }
}