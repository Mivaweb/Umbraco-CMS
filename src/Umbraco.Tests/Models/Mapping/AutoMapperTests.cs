using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Moq;
using NUnit.Framework;
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;
using Umbraco.Core.Manifest;
using Umbraco.Core.PropertyEditors;
using Umbraco.Tests.TestHelpers;
using Umbraco.Core.DI;

namespace Umbraco.Tests.Models.Mapping
{
    [RequiresAutoMapperMappings]
    [TestFixture]
    public class AutoMapperTests : TestWithApplicationBase
    {
        protected override void Compose()
        {
            base.Compose();

            var manifestBuilder = new ManifestBuilder(
                CacheHelper.CreateDisabledCacheHelper().RuntimeCache,
                new ManifestParser(Logger, new DirectoryInfo(TestHelper.CurrentAssemblyDirectory), CacheHelper.CreateDisabledCacheHelper().RuntimeCache));
            Container.Register(_ => manifestBuilder);

            Func<IEnumerable<Type>> typeListProducerList = Enumerable.Empty<Type>;
            Container.RegisterCollectionBuilder<PropertyEditorCollectionBuilder>()
                .Add(typeListProducerList);
        }

        [Test]
        public void Assert_Valid_Mappings()
        {
            Mapper.AssertConfigurationIsValid();
        }
    }
}