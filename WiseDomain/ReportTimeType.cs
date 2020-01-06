namespace WiseDomain
{
    public enum ReportTimeType
    {
        /// <summary>
        /// Time parameter didn't used
        /// </summary>
        None,
        /// <summary>
        /// Report created between `From` until `To` datetimes
        /// </summary>
        Between,
        /// <summary>
        /// Report created from last N seconds until now
        /// </summary>
        Last,
    }
}