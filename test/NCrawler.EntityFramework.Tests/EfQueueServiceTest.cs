﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NCrawler.Interfaces;
using NCrawler.Test;
using NUnit.Framework;

namespace NCrawler.EntityFramework.Tests
{
    [TestFixture]
    public class EfQueueServiceTest : QueueServiceTestBase
    {
        protected override ICrawlerQueue GetCrawlQueue()
        {
            return new EfCrawlQueueService(new Uri("http://www.ncrawler.com"), false);
        }
    }
}
