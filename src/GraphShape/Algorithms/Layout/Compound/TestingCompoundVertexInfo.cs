namespace GraphShape.Algorithms.Layout
{
    /// <summary>
    /// Compound vertex extra information.
    /// </summary>
    public class TestingCompoundVertexInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestingCompoundVertexInfo"/> class.
        /// </summary>
        /// <param name="springForce">Spring force.</param>
        /// <param name="repulsionForce">Repulsion force.</param>
        /// <param name="gravityForce">Gravity force.</param>
        /// <param name="applicationForce">Application force.</param>
        public TestingCompoundVertexInfo(
            Vector springForce,
            Vector repulsionForce,
            Vector gravityForce,
            Vector applicationForce)
        {
            SpringForce = springForce;
            RepulsionForce = repulsionForce;
            GravityForce = gravityForce;
            ApplicationForce = applicationForce;
        }

        /// <summary>
        /// Spring force.
        /// </summary>
        public Vector SpringForce { get; set; }
        
        /// <summary>
        /// Repulsion force.
        /// </summary>
        public Vector RepulsionForce { get; set; }
        
        /// <summary>
        /// Gravity force.
        /// </summary>
        public Vector GravityForce { get; set; }
        
        /// <summary>
        /// Application force.
        /// </summary>
        public Vector ApplicationForce { get; set; }
    }
}
