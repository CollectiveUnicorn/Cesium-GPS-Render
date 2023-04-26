using System.Collections.Generic;

public interface IDataService {

	public abstract IEnumerable<TrackedEntity> GetTrackedEntities();

	public abstract IEnumerable<TrackedEntity> GetTrackedEntitiesByName(string name);

	public abstract TrackedEntity GetTrackedEntity(string name);

	public abstract TrackedEntity CreateTrackedEntity(string name, double latitude, double longitude, double height);
}