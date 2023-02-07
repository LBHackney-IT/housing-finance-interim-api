using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.Dsl;
using Bogus;
using Google.Apis.Drive.v3.Data;
using HousingFinanceInterimApi.V1.Domain;

namespace HousingFinanceInterimApi.Tests.V1.TestHelpers
{
    public static class RandomGen
    {
        private static readonly Faker _faker = new Faker();
        private static readonly Fixture _fixture = new Fixture();

        public static int Id(int maxId = 10000)
            => _faker.Random.Int(0, maxId);

        public static int WholeNumber(int min = int.MinValue, int max = int.MinValue)
            => _faker.Random.Int(min, max);

        public static TItem Create<TItem>() => _fixture.Create<TItem>();
        public static ICustomizationComposer<TItem> Build<TItem>() => _fixture.Build<TItem>();
        public static IEnumerable<TItem> CreateMany<TItem>(int quantity = 3) => _fixture.CreateMany<TItem>(quantity);

        public static IEnumerable<TItem> CreateMany<TItem>(Func<TItem> creatorDelegate, int itemCount)
            => Enumerable.Range(1, itemCount).Select(_ => creatorDelegate()).ToList();

        // The "Do" saves 1 step of assignment by converting to PostProcessComposer immediately.
        private static IPostprocessComposer<T> BuildConditionally<T>() where T : class
            => _fixture.Build<T>().Do((_) => { });

        public static BatchLogDomain BatchLogDomain(bool withErrorLogs = false)
        {
            var batchLogBuilder = BuildConditionally<BatchLogDomain>();

            if (withErrorLogs)
                batchLogBuilder = batchLogBuilder.Without(b => b.BatchLogErrors);

            return batchLogBuilder.Create();
        }

        public static IEnumerable<File> GoogleDriveFiles(bool filesValidity, int count = 3)
            => CreateMany(() => GoogleDriveFile(filesValidity), count);

        private static File GoogleDriveFile(bool isValidFile)
        {
            var gDriveFileBuilder = BuildConditionally<File>();

            if (isValidFile)
            {
                var dateDiffBetweenDates1And2 = WholeNumber(8, 14);
                var academyFileName = AcademyFileName(dateDiffBetweenDates1And2);
                gDriveFileBuilder = gDriveFileBuilder.With(f => f.Name, academyFileName);
            }
            
            return gDriveFileBuilder.Create();
        }

        /*
            According to the MoveFileUC code, these are the possible scenarios:
            1. The Date1 is the Past day & Date2 is the Now day
            2. The Date1 is the Past day & Date 2 is the Future day
            3. The Date1 is the Now day & Date 2 is the Future day.
            4,5,6. Same scenarios, but Date1 is Date2 & vice versa.

            Until I can see the file names & their creation dates, I will stick with scenario 3.
            With an assumptions that:
            1. The Date 2 is future date stating when the file is supposed to be in effect
            2. we subtract 7 days because we want to prep the process a week in advance. 
        */
        private static string AcademyFileName(int dayRange)
        {
            var date1 = DateTime.UtcNow.ToString("ddMMyyyy");
            var date2 = DateTime.UtcNow.AddDays(dayRange).ToString("ddMMyyyy");
            return $"{date1}_Something_Academy_{date2}"; // TODO: change to realistic format
        }
    }
}
