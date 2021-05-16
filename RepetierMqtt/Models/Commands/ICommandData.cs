namespace RepetierMqtt.Models.Commands
{
    public interface ICommandData
    {
        public string CommandIdentifier { get; }
    }

    /*
     * 
     * commands:
     * 
     * getDialogs { "lang": "de"}
     * setLogLevel { "level": 12} 
     * activePrinter { "printer": "EVOlizer"}
     * getExcludeRegions {} 
     * addExcludeRegion {xmin: 197.30769230769232, ymin: 66.63879598662203, xmax: 251.6387959866221, ymax: 157.19063545150502}
     * updateAvailable {}
     * networkInterfaces {} 
     * freeSpace {} 
     * sendMoves {}
     * hideMoves {}
     * delExcludeRegion { id: 1, xmax: 149.6488342285156, xmin: 92.45819091796875, ymax: 127.6421432495117, ymin: 78.07691955566406}
     * move { "x": -10, "relative": true }
     * motorsOff {} 
     * setFanSpeed { "speed": 255, "on": true, "fanId": 0}
     * setFilterFan {"filter": true}
     * deactivate { "printer": "EVOlizer" }
     * activate  { "printer": "EVOlizer" }
     * historySummary {year: 2021, slug: "EVOlizer", allPrinter: false}
     * historyList {start: 0, limit: 50, slug: "EVOlizer", uuid: "", allPrinter: false, page: 0} 
     * getScript { "name": "start"}  one of: [start, end, kill, pause, continue, connect, shutdown, remove_filament, insert_filament, before_snapshot, after_snapshot]
     * 
     * 
     * events: 
     * 
     * excludeRegionsChanged {} 
     * 
     * 
     * 
     */
}