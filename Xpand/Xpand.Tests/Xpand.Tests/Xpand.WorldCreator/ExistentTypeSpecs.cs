﻿using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.Persistent.BaseImpl.PersistentMetaData;
using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;

using Machine.Specifications;
using TypeMock.ArrangeActAssert;

namespace Xpand.Tests.Xpand.WorldCreator
{
    [Subject(typeof(ExistentTypesMemberCreator))]
    [Isolated]
    public class When_Creating_ExistentTypes_CoreMembers_that_exist_already:With_In_Memory_DataStore {
        Establish context = () =>{
            var memberInfo = ObjectSpace.CreateObject<ExtendedCoreTypeMemberInfo>();
            memberInfo.DataType = DBColumnType.String;
            memberInfo.Name = "UserName";
            memberInfo.Owner = typeof(User);
            memberInfo.TypeAttributes.Add(new PersistentSizeAttribute(memberInfo.Session));                
            UnitOfWork.CommitChanges();            
        };

        Because of = () => new ExistentTypesMemberCreator().CreateMembers(UnitOfWork);


        It should_not_throw_any_exceptions = () => XafTypesInfo.XpoTypeInfoSource.XPDictionary.GetClassInfo(typeof(User)).FindMember("UserName");
    }

    [Subject(typeof(ExistentTypesMemberCreator))]
    [Isolated]
    public class When_Creating_ExistentTypes_CoreMembers :With_In_Memory_DataStore
    {
        
        static XPMemberInfo memberInfo;


        Establish context = () => {
            var typeMemberInfo = ObjectSpace.CreateObject<ExtendedCoreTypeMemberInfo>();
            typeMemberInfo.DataType = DBColumnType.Boolean;
            typeMemberInfo.Name = "Test";
            typeMemberInfo.Owner = typeof(User);
            typeMemberInfo.TypeAttributes.Add(new PersistentSizeAttribute(typeMemberInfo.Session));                
            UnitOfWork.CommitChanges();
        };

        Because of = () => new ExistentTypesMemberCreator().CreateMembers(UnitOfWork);

        It should_find_that_member_through_xpdictionary =
            () => {
                memberInfo = XafTypesInfo.XpoTypeInfoSource.XPDictionary.GetClassInfo(typeof(User)).FindMember("Test");
                memberInfo.ShouldNotBeNull();
            };

        It should_create_typedattributes =
            () => memberInfo.FindAttributeInfo(typeof (SizeAttribute)).ShouldNotBeNull();
    }

    [Subject(typeof(ExistentTypesMemberCreator))]
    public class When_Creating_ExistentTypes_ReferenceMembers : With_In_Memory_DataStore
    {
        
        static XPMemberInfo memberInfo;

        Establish context = () => {
            var referenceMemberInfo = ObjectSpace.CreateObject<ExtendedReferenceMemberInfo>();
            referenceMemberInfo.Name = "Test";
            referenceMemberInfo.Owner = typeof(User);
            referenceMemberInfo.ReferenceType = typeof(Role);
            referenceMemberInfo.TypeAttributes.Add(new PersistentAssociationAttribute(referenceMemberInfo.Session) { AssociationName = "ANAME", ElementType = typeof(User) });

            UnitOfWork.CommitChanges();            
        };

        Because of = () => new ExistentTypesMemberCreator().CreateMembers(UnitOfWork);

        It should_find_that_member_through_xpdictionary =
            () => {
                memberInfo = XafTypesInfo.XpoTypeInfoSource.XPDictionary.GetClassInfo(typeof(User)).FindMember("Test");
                memberInfo.ShouldNotBeNull();
            };

        It should_create_typedattributes =
            () => memberInfo.FindAttributeInfo(typeof (AssociationAttribute)).ShouldNotBeNull();
    }

    [Subject(typeof(ExistentTypesMemberCreator))]
    [Isolated]
    public class When_Creating_ExistentTypes_CollectionMembers : With_In_Memory_DataStore
    {
        static XPMemberInfo memberInfo;

        Establish context = () => {
            var collectionMemberInfo = ObjectSpace.CreateObject<ExtendedCollectionMemberInfo>();
            collectionMemberInfo.Name = "Test";
            collectionMemberInfo.Owner = typeof(User);
            collectionMemberInfo.TypeAttributes.Add(new PersistentSizeAttribute(collectionMemberInfo.Session));                
            UnitOfWork.CommitChanges();            
        };

        Because of = () => new ExistentTypesMemberCreator().CreateMembers(UnitOfWork);

        It should_find_that_member_through_xpdictionary =
            () => {
                memberInfo = XafTypesInfo.XpoTypeInfoSource.XPDictionary.GetClassInfo(typeof(User)).FindMember("Test");
                memberInfo.ShouldNotBeNull();
            };

        It should_create_typedattributes =
            () => memberInfo.FindAttributeInfo(typeof (SizeAttribute)).ShouldNotBeNull();
    }

}
