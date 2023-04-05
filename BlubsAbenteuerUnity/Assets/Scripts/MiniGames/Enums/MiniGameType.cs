// type of the mini game
public enum MiniGameType
{
    INSERT, // insert 1...n number(s) into a given row of m numbers
    COUNT, // pick numbers (incr/decr)
    PAIRS, // connect pairs of same sets/ sets and numbers
    ADD, // connect pairs that add to a distinct number
    MEMORY, // memory
    CONNECT, // connect sum/div of two sets/numbers with result
    MEMORY_VS,   // memory, but for 2 players
    COUNT_VS,    // count, but for 2 players
    CONNECT_VS  // connect, but for 2 players
}
