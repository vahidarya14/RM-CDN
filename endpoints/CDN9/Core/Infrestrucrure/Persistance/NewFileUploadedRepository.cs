using CDN9.Core.Domain;
using LiteDB;
using System.Linq.Expressions;

namespace CDN9.Core.Infrestrucrure.Persistance;

public class NewFileUploadedRepository
{
    public static string Db => "MyData.db";

    public void Insert(params NewFileUploaded[] newFileUploadeds)
    {
        try
        {
            // Open database (or create if doesn't exist)
            using var db = new LiteDatabase(Db);
            var table = db.GetCollection<NewFileUploaded>($"{nameof(NewFileUploaded)}s");

            table.EnsureIndex(x => new { x.Name, x.CndHost }, true);

            foreach (var item in newFileUploadeds)
            {
                try
                {
                    table.Insert(item);
                }
                catch (Exception)
                {

                    throw;
                }
            }
       }

        catch (Exception ex)
        {

            throw;
        }

    }

    public NewFileUploaded? FindOne(Expression<Func<NewFileUploaded, bool>> predicate)
    {
        using var db = new LiteDatabase(Db);

        var table = db.GetCollection<NewFileUploaded>($"{nameof(NewFileUploaded)}s");
        return table.FindOne(predicate);
    }

    public BsonValue Update(NewFileUploaded entity)
    {
        using var db = new LiteDatabase(Db);

        var table = db.GetCollection<NewFileUploaded>($"{nameof(NewFileUploaded)}s");
        return table.Update(entity);
    }

    public BsonValue Delete(NewFileUploaded entity)
    {
        using var db = new LiteDatabase(Db);

        var table = db.GetCollection<NewFileUploaded>($"{nameof(NewFileUploaded)}s");
        return table.Delete(entity.Id);
    }
    
}
