using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ActivityReceiver.Enums
{
    public enum MovementState
    {
        // Cases for the behavior dragging single WordItem
        DragSingleBegin = 0,
        DragSingleMove = 1,
        DragSingleEnd = 2,

        // Cases for the behavior making group for multiple WordItem
        MakeGroupBegin = 3,
        MakeGroupMove = 4,
        MakeGroupEnd = 5,

        // Cases for the behavior dragging multiple WordItem when they are selected
        DragGroupBegin = 6,
        DragGroupMove = 7,
        DragGroupEnd = 8,

        // Cancel group
        CancelGroup = 9,
    }
}
