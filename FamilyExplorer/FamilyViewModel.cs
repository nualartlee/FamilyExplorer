﻿/* 
Family Explorer - Record and View Family Relationships
Copyright(C) 2016  Javier Nualart Lee (nualartlee@yahoo.com)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License version 3 as
published by the Free Software Foundation.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.If not, see<http://www.gnu.org/licenses/> */
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Serialization;

namespace FamilyExplorer
{
    public class FamilyViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                if (value != title)
                {
                    title = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ObservableCollection<Person> members;
        public ObservableCollection<Person> Members
        {
            get { return members; }
            set
            {
                if (value != members)
                {
                    members = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ObservableCollection<Relationship> relationships;
        public ObservableCollection<Relationship> Relationships
        {
            get { return relationships; }
            set
            {
                if (value != relationships)
                {
                    relationships = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Tree tree;
        public Tree Tree
        {
            get { return tree; }
            set
            {
                if (value != tree)
                {
                    tree = value;
                    NotifyPropertyChanged();
                }
            }
        }        

        private Cursor familyTreeCursor;
        public Cursor FamilyTreeCursor
        {
            get { return familyTreeCursor; }
            set
            {
                if (value != familyTreeCursor)
                {
                    familyTreeCursor = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string setCommandInProgressDescription;
        public string SetCommandInProgressDescription
        {
            get { return setCommandInProgressDescription; }
            set
            {
                if (value != setCommandInProgressDescription)
                {
                    setCommandInProgressDescription = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool setCommandInProgress;
        public bool SetCommandInProgress
        {
            get { return setCommandInProgress; }
            set
            {
                if (value != setCommandInProgress)
                {
                    setCommandInProgress = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int setCommandInProgressType;
        private Person setCommandTargetPerson;

        public FamilyViewModel()
        {

        }

        public void CreateNewFamily()
        {            
            Tree = new Tree();
            Members = new ObservableCollection<Person> { };
            Relationships = new ObservableCollection<Relationship> { };
            Person person = new Person();
            InitalizePerson(person);
            AddPersonToFamily(person);
            Tree.Scale = 1;
            CenterTreeInWindow();
            FamilyTreeCursor = Cursors.Arrow;
            setCommandInProgressType = 0;
            Title = "Family Explorer - NewFamily.fex";
        }

        #region Commands

        public void AddMother_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            Person person = (Person)e.Parameter;
            if (person.MotherId == 0)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        public void AddMother_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Person person = (Person)e.Parameter;
            AddMotherToPerson(person);
        }

        private void AddMotherToPerson(Person child)
        {
            Person mom = new Person();
            InitalizePerson(mom);
            mom.FirstName = "Mother Of " + child.FirstName;
            mom.LastName = "";
            mom.Gender = "Female";
            mom.GenerationIndex = child.GenerationIndex - 1;
            mom.ChildrenIds.Add(child.Id);

            child.MotherId = mom.Id;

            AddPersonToFamily(mom);

            
        }

        public void AddFather_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            Person person = (Person)e.Parameter;
            if (person.FatherId == 0)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        public void AddFather_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Person person = (Person)e.Parameter;
            AddFatherToPerson(person);
        }

        private void AddFatherToPerson(Person child)
        {
            Person dad = new Person();
            InitalizePerson(dad);
            dad.FirstName = "Father Of " + child.FirstName;
            dad.LastName = "";
            dad.Gender = "Male";
            dad.GenerationIndex = child.GenerationIndex - 1;
            dad.ChildrenIds.Add(child.Id);

            child.FatherId = dad.Id;

            AddPersonToFamily(dad);

            
        }

        public void AddSiblingEqualParents_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void AddSiblingEqualParents_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Person person = (Person)e.Parameter;
            AddSiblingEqualParentsToPerson(person);
        }

        private void AddSiblingEqualParentsToPerson(Person person)
        {
            // Create new sibling
            Person newSibling = new Person();
            InitalizePerson(newSibling);
            newSibling.FirstName = "Sibling Of " + person.FirstName;
            newSibling.LastName = "";
            newSibling.Gender = "Not Specified";
            newSibling.GenerationIndex = person.GenerationIndex;
            // Add current siblings to new sibling's list
            newSibling.SiblingIds.Add(person.Id);
            foreach (int siblingid in person.SiblingIds)
            {
                newSibling.SiblingIds.Add(siblingid);
            }
            // Add parents to new sibling
            newSibling.MotherId = person.MotherId;
            newSibling.FatherId = person.FatherId;
            // Add new sibling to all other siblings' lists
            foreach (int siblingid in newSibling.SiblingIds)
            {
                Person otherSibling = getPerson(siblingid);
                if (otherSibling != null)
                {
                    otherSibling.SiblingIds.Add(newSibling.Id);
                }
            }
            // Add new sibling to mother's children
            Person mom = getPerson(person.MotherId);
            if (mom != null)
            {
                mom.ChildrenIds.Add(newSibling.Id);
            }
            // Add new sibling to father's children
            Person dad = getPerson(person.FatherId);
            if (dad != null)
            {
                dad.ChildrenIds.Add(newSibling.Id);
            }

            AddPersonToFamily(newSibling);

        }

        public void AddSiblingByMother_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void AddSiblingByMother_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Person person = (Person)e.Parameter;
            AddSiblingByMotherToPerson(person);
        }

        private void AddSiblingByMotherToPerson(Person person)
        {
            // Create new sibling
            Person newSibling = new Person();
            InitalizePerson(newSibling);
            newSibling.FirstName = "Sibling Of " + person.FirstName;
            newSibling.LastName = "";
            newSibling.Gender = "Not Specified";
            newSibling.GenerationIndex = person.GenerationIndex;
            // Add current siblings to new sibling's list    
            Person mom = getPerson(person.MotherId);
            if (mom != null)
            {
                foreach (int siblingid in mom.ChildrenIds)
                {
                    newSibling.SiblingIds.Add(siblingid);
                }
            }
            else
            {
                newSibling.SiblingIds.Add(person.Id);
            }
            // Add mother to new sibling
            newSibling.MotherId = person.MotherId;
            // Add new sibling to all other siblings' lists
            foreach (int siblingid in newSibling.SiblingIds)
            {
                Person otherSibling = getPerson(siblingid);
                if (otherSibling != null)
                {
                    otherSibling.SiblingIds.Add(newSibling.Id);
                }
            }
            // Add new sibling to mother's children    
            if (mom != null)
            {
                mom.ChildrenIds.Add(newSibling.Id);
            }

            AddPersonToFamily(newSibling);

        }

        public void AddSiblingByFather_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void AddSiblingByFather_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Person person = (Person)e.Parameter;
            AddSiblingByFatherToPerson(person);
        }

        private void AddSiblingByFatherToPerson(Person person)

        {
            // Create new sibling
            Person newSibling = new Person();
            InitalizePerson(newSibling);
            newSibling.FirstName = "Sibling Of " + person.FirstName;
            newSibling.LastName = "";
            newSibling.Gender = "Not Specified";
            newSibling.GenerationIndex = person.GenerationIndex;
            // Add current siblings to new sibling's list    
            Person dad = getPerson(person.FatherId);
            if (dad != null)
            {
                foreach (int siblingid in dad.ChildrenIds)
                {
                    newSibling.SiblingIds.Add(siblingid);
                }
            }
            else
            {
                newSibling.SiblingIds.Add(person.Id);
            }
            // Add father to new sibling            
            newSibling.FatherId = person.FatherId;
            // Add new sibling to all other siblings' lists
            foreach (int siblingid in newSibling.SiblingIds)
            {
                Person otherSibling = getPerson(siblingid);
                if (otherSibling != null)
                {
                    otherSibling.SiblingIds.Add(newSibling.Id);
                }
            }
            // Add new sibling to father's children     
            if (dad != null)
            {
                dad.ChildrenIds.Add(newSibling.Id);
            }

            AddPersonToFamily(newSibling);

        }

        public void AddFriend_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void AddFriend_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Person person = (Person)e.Parameter;
            AddFriendToPerson(person);
        }

        private void AddFriendToPerson(Person person)
        {
            Person friend = new Person();
            InitalizePerson(friend);
            friend.FirstName = "Friend Of " + person.FirstName;
            friend.LastName = "";
            friend.Gender = "Not Specified";
            friend.GenerationIndex = person.GenerationIndex;
            friend.FriendIds.Add(person.Id);

            person.FriendIds.Add(friend.Id);

            AddPersonToFamily(friend);            
        }

        public void AddPartner_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void AddPartner_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Person person = (Person)e.Parameter;
            AddPartnerToPerson(person);
        }

        private void AddPartnerToPerson(Person person)
        {
            Person partner = new Person();
            InitalizePerson(partner);
            partner.FirstName = "Partner Of " + person.FirstName;
            partner.LastName = "";
            partner.Gender = "Not Specified";
            partner.GenerationIndex = person.GenerationIndex;
            partner.PartnerIds.Add(person.Id);

            person.PartnerIds.Add(partner.Id);

            AddPersonToFamily(partner);            
        }

        public void AddChild_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void AddChild_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Person person = (Person)e.Parameter;
            AddChildToPerson(person);
        }

        private void AddChildToPerson(Person parent)

        {
            // Create new child
            Person newChild = new Person();
            InitalizePerson(newChild);
            newChild.FirstName = "Child Of " + parent.FirstName;
            newChild.LastName = "";
            newChild.Gender = "Not Specified";
            newChild.GenerationIndex = parent.GenerationIndex + 1;
            // Add parent to new child   
            if (parent.Gender == "Male")
            {
                newChild.FatherId = parent.FatherId;
            }
            if (parent.Gender == "Female")
            {
                newChild.MotherId = parent.MotherId;
            }
            // Add current children to new child's sibling list                
            foreach (int childId in parent.ChildrenIds)
            {
                newChild.SiblingIds.Add(childId);
            }
            // Add new child to all other childrens' sibling lists
            foreach (int siblingid in newChild.SiblingIds)
            {
                Person otherSibling = getPerson(siblingid);
                if (otherSibling != null)
                {
                    otherSibling.SiblingIds.Add(newChild.Id);
                }
            }
            // Add new child to parent  
            parent.ChildrenIds.Add(newChild.Id);

            AddPersonToFamily(newChild);

        }

        public void AddAbuser_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void AddAbuser_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Person person = (Person)e.Parameter;
            AddAbuserToPerson(person);
        }

        private void AddAbuserToPerson(Person victim)
        {
            Person abuser = new Person();
            InitalizePerson(abuser);
            abuser.FirstName = "Abuser Of " + abuser.FirstName;
            abuser.LastName = "";
            abuser.Gender = "Not Specified";
            abuser.GenerationIndex = abuser.GenerationIndex - 1;
            abuser.VictimIds.Add(abuser.Id);

            abuser.AbuserIds.Add(abuser.Id);

            AddPersonToFamily(abuser);            
        }

        public void AddVictim_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void AddVictim_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Person person = (Person)e.Parameter;
            AddVictimToPerson(person);
        }

        private void AddVictimToPerson(Person abuser)
        {
            Person victim = new Person();
            InitalizePerson(victim);
            victim.FirstName = "Victim Of " + abuser.FirstName;
            victim.LastName = "";
            victim.Gender = "Not Specified";
            victim.GenerationIndex = abuser.GenerationIndex + 1;
            victim.AbuserIds.Add(abuser.Id);

            abuser.VictimIds.Add(victim.Id);

            AddPersonToFamily(victim);            
        }

        public void SetMother_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            Person person = (Person)e.Parameter;
            if (person.MotherId > 0) { e.CanExecute = false; }
            else { e.CanExecute = true; }
        }

        public void SetMother_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            setCommandTargetPerson = (Person)e.Parameter;
            setCommandInProgressType = 1;
            SetCommandInProgressDescription = "Select " + setCommandTargetPerson.FirstName + "'s Mother";
            SetCommandInProgress = true;
        }

        private bool SetMother_CanFinalize(Person person)
        {
            // Not in previous generation
            if (person.GenerationIndex != setCommandTargetPerson.GenerationIndex - 1) { return false; }
            // Not female
            if (person.Gender != "Female") { return false; }
            return true;
        }

        private void SetMother_Finalized(Person person, Person mother)
        {

            if (mother.Gender != "Female") { return; }
            // Add mother to person                          
            person.MotherId = mother.Id;
            // Add mother's current children to the person's sibling list                
            foreach (int childId in mother.ChildrenIds)
            {
                person.SiblingIds.Add(childId);
            }
            // Add person to mother's other childrens' sibling lists
            foreach (int siblingid in person.SiblingIds.ToList())
            {
                Person otherSibling = getPerson(siblingid);
                if (otherSibling != null)
                {
                    otherSibling.SiblingIds.Add(person.Id);
                }
            }
            // Add person to mother  
            mother.ChildrenIds.Add(person.Id);
        }

        public void SetFather_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            Person person = (Person)e.Parameter;
            if (person.FatherId > 0) { e.CanExecute = false; }
            else { e.CanExecute = true; }
        }

        public void SetFather_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            setCommandTargetPerson = (Person)e.Parameter;
            setCommandInProgressType = 2;
            SetCommandInProgressDescription = "Select " + setCommandTargetPerson.FirstName + "'s Father";
            SetCommandInProgress = true;
        }

        private bool SetFather_CanFinalize(Person person)
        {
            // Not in previous generation
            if (person.GenerationIndex != setCommandTargetPerson.GenerationIndex - 1) { return false; }
            // Not male
            if (person.Gender != "Male") { return false; }
            return true;
        }

        private void SetFather_Finalized(Person person, Person father)
        {

            if (father.Gender != "Male") { return; }
            // Add father to person                          
            person.FatherId = father.Id;
            // Add father's current children to the person's sibling list                
            foreach (int childId in father.ChildrenIds)
            {
                person.SiblingIds.Add(childId);
            }
            // Add person to father's other childrens' sibling lists
            foreach (int siblingid in person.SiblingIds)
            {
                Person otherSibling = getPerson(siblingid);
                if (otherSibling != null)
                {
                    otherSibling.SiblingIds.Add(person.Id);
                }
            }
            // Add person to father  
            father.ChildrenIds.Add(person.Id);            
        }

        public void SetFriend_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void SetFriend_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            setCommandTargetPerson = (Person)e.Parameter;
            setCommandInProgressType = 3;
            SetCommandInProgressDescription = "Select " + setCommandTargetPerson.FirstName + "'s Friend";
            SetCommandInProgress = true;
        }

        private bool SetFriend_CanFinalize(Person person)
        {
            // Not itself
            if (person == setCommandTargetPerson) { return false; }
            // Not already a friend
            if (person.FriendIds.Contains(setCommandTargetPerson.Id)) { return false; }
            return true;
        }

        private void SetFriend_Finalized(Person person, Person partner)
        {
            // Add friend to person's friends list                        
            person.FriendIds.Add(partner.Id);
            // Add person to friend's friend list  
            partner.FriendIds.Add(person.Id);
        }

        public void SetPartner_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void SetPartner_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            setCommandTargetPerson = (Person)e.Parameter;
            setCommandInProgressType = 4;
            SetCommandInProgressDescription = "Select " + setCommandTargetPerson.FirstName + "'s Partner";
            SetCommandInProgress = true;
        }

        private bool SetPartner_CanFinalize(Person person)
        {
            // Not itself
            if (person == setCommandTargetPerson) { return false; }
            // Not already a partner
            if (person.PartnerIds.Contains(setCommandTargetPerson.Id)) { return false; }
            return true;
        }

        private void SetPartner_Finalized(Person person, Person partner)
        {
            // Add partner to person's partner list                        
            person.PartnerIds.Add(partner.Id);
            // Add person to partners' partner list  
            partner.PartnerIds.Add(person.Id);
        }

        public void SetChild_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            Person person = (Person)e.Parameter;
            if (person.Gender == "Male" || person.Gender == "Female")
            { e.CanExecute = true; }
            else { e.CanExecute = false; }
        }

        public void SetChild_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            setCommandTargetPerson = (Person)e.Parameter;
            setCommandInProgressType = 5;
            SetCommandInProgressDescription = "Select " + setCommandTargetPerson.FirstName + "'s Child";
            SetCommandInProgress = true;
        }

        private bool SetChild_CanFinalize(Person child)
        {
            // Not already a child
            if (setCommandTargetPerson.ChildrenIds.Contains(child.Id)) { return false; }
            // Not in the next generation
            if (setCommandTargetPerson.GenerationIndex + 1 != child.GenerationIndex) { return false; }
            return true;
        }

        private void SetChild_Finalized(Person person, Person child)
        {
            // Add siblings to child
            foreach (int childId in person.ChildrenIds)
            {
                if (!child.SiblingIds.Contains(childId))
                {
                    child.SiblingIds.Add(childId);
                }
            }
            // add child to siblings
            foreach (int siblingid in person.SiblingIds)
            {
                Person otherSibling = getPerson(siblingid);
                if (otherSibling != null)
                {
                    if (!otherSibling.SiblingIds.Contains(child.Id))
                    {
                        otherSibling.SiblingIds.Add(child.Id);
                    }
                }
            }
            // Add child to person's children list                        
            person.ChildrenIds.Add(child.Id);
            // Add person as child's parent
            if (person.Gender == "Male") { child.FatherId = person.Id; }
            if (person.Gender == "Female") { child.MotherId = person.Id; }

        }

        public void SetAbuser_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void SetAbuser_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            setCommandTargetPerson = (Person)e.Parameter;
            setCommandInProgressType = 6;
            SetCommandInProgressDescription = "Select " + setCommandTargetPerson.FirstName + "'s Abuser";
            SetCommandInProgress = true;
        }

        private bool SetAbuser_CanFinalize(Person person)
        {
            // Not already an abuser
            if (person.VictimIds.Contains(setCommandTargetPerson.Id)) { return false; }
            return true;
        }

        private void SetAbuser_Finalized(Person person, Person abuser)
        {
            // Add abuser to person's abuser list                        
            person.AbuserIds.Add(abuser.Id);
            // Add person to abuser's victim list  
            abuser.VictimIds.Add(person.Id);
        }

        public void SetVictim_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void SetVictim_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            setCommandTargetPerson = (Person)e.Parameter;
            setCommandInProgressType = 7;
            SetCommandInProgressDescription = "Select " + setCommandTargetPerson.FirstName + "'s Victim";
            SetCommandInProgress = true;
        }

        private bool SetVictim_CanFinalize(Person person)
        {
            // Not already a victim
            if (person.AbuserIds.Contains(setCommandTargetPerson.Id)) { return false; }
            return true;
        }

        private void SetVictim_Finalized(Person person, Person victim)
        {
            // Add victim to person's victim list                        
            person.VictimIds.Add(victim.Id);
            // Add person to victim's abuser list  
            victim.AbuserIds.Add(person.Id);
        }

        public void FinalizeSetCommand(Person setCommandRelationPerson)
        {
            if (SetCommandInProgress)
            {
                switch (setCommandInProgressType)
                {
                    case 1: // Set mother
                        if (SetMother_CanFinalize(setCommandRelationPerson))
                        { SetMother_Finalized(setCommandTargetPerson, setCommandRelationPerson); }
                        break;
                    case 2: // Set father
                        if (SetFather_CanFinalize(setCommandRelationPerson))
                        { SetFather_Finalized(setCommandTargetPerson, setCommandRelationPerson); }
                        break;
                    case 3: // Set friend
                        if (SetFriend_CanFinalize(setCommandRelationPerson))
                        { SetFriend_Finalized(setCommandTargetPerson, setCommandRelationPerson); }
                        break;
                    case 4: // Set partner
                        if (SetPartner_CanFinalize(setCommandRelationPerson))
                        { SetPartner_Finalized(setCommandTargetPerson, setCommandRelationPerson); }
                        break;
                    case 5: // Set child
                        if (SetChild_CanFinalize(setCommandRelationPerson))
                        { SetChild_Finalized(setCommandTargetPerson, setCommandRelationPerson); }
                        break;
                    case 6: // Set abuser
                        if (SetAbuser_CanFinalize(setCommandRelationPerson))
                        { SetAbuser_Finalized(setCommandTargetPerson, setCommandRelationPerson); }
                        break;
                    case 7: // Set victim
                        if (SetVictim_CanFinalize(setCommandRelationPerson))
                        { SetVictim_Finalized(setCommandTargetPerson, setCommandRelationPerson); }
                        break;
                    default:
                        break;
                }
            }
            EndSetCommand();
        }

        public void EndSetCommand()
        {
            if (SetCommandInProgress)
            {
                setCommandTargetPerson = null;
                setCommandInProgressType = 0;
                SetCommandInProgressDescription = "";
                SetCommandInProgress = false;
                FamilyTreeCursor = Cursors.Arrow;
                ResetAllRelationships();
            }
            
        }

        public void EnterSetCommandRelation(Person person)
        {

            switch (setCommandInProgressType)
            {
                case 0: // No command in progress
                    FamilyTreeCursor = Cursors.Arrow;
                    break;
                case 1: // Set mother
                    if (SetMother_CanFinalize(person))
                    { FamilyTreeCursor = Cursors.Hand; }
                    else { FamilyTreeCursor = Cursors.No; }
                    break;
                case 2: // Set father
                    if (SetFather_CanFinalize(person))
                    { FamilyTreeCursor = Cursors.Hand; }
                    else { FamilyTreeCursor = Cursors.No; }
                    break;
                case 3: // Set friend
                    if (SetFriend_CanFinalize(person))
                    { FamilyTreeCursor = Cursors.Hand; }
                    else { FamilyTreeCursor = Cursors.No; }
                    break;
                case 4: // Set partner
                    if (SetPartner_CanFinalize(person))
                    { FamilyTreeCursor = Cursors.Hand; }
                    else { FamilyTreeCursor = Cursors.No; }
                    break;
                case 5: // Set child
                    if (SetChild_CanFinalize(person))
                    { FamilyTreeCursor = Cursors.Hand; }
                    else { FamilyTreeCursor = Cursors.No; }
                    break;
                case 6: // Set abuser
                    if (SetAbuser_CanFinalize(person))
                    { FamilyTreeCursor = Cursors.Hand; }
                    else { FamilyTreeCursor = Cursors.No; }
                    break;
                case 7: // Set victim
                    if (SetVictim_CanFinalize(person))
                    { FamilyTreeCursor = Cursors.Hand; }
                    else { FamilyTreeCursor = Cursors.No; }
                    break;
                default:
                    FamilyTreeCursor = Cursors.Arrow;
                    break;
            }
        }

        public void ExitSetCommandRelation()
        {
            if (SetCommandInProgress) { FamilyTreeCursor = Cursors.Arrow; }
            else { FamilyTreeCursor = Cursors.Arrow; }
        }

        #endregion Commands

        private void ResetAllRelationships()
        {
            Relationships = new ObservableCollection<Relationship> { };
            foreach (Person person in Members)
            {
                ResetPersonRelationships(person);
            }
        }

        private void ResetPersonRelationships(Person person)
        {

            // Mother
            if (person.MotherId > 0)
            {
                Person mom = getPerson(person.MotherId);
                ResetRelationship(1, mom, person, person.DOB, null);
            }

            // Father
            if (person.FatherId > 0)
            {
                Person dad = getPerson(person.FatherId);
                ResetRelationship(2, dad, person, person.DOB, null);
            }

            // Siblings
            foreach (int siblingId in person.SiblingIds)
            {
                Person sibling = getPerson(siblingId);
                Person sourcePerson = (person.Id > sibling.Id) ? person : sibling;
                Person destinationPerson = (person.Id > sibling.Id) ? sibling : person;
                DateTime startDate = (person.DOB < sibling.DOB) ? person.DOB : sibling.DOB;
                ResetRelationship(3, sourcePerson, destinationPerson, startDate, null);
            }

            // Friends
            foreach (int friendId in person.FriendIds)
            {
                Person friend = getPerson(friendId);
                Person sourcePerson = (person.Id > friend.Id) ? person : friend;
                Person destinationPerson = (person.Id > friend.Id) ? friend : person;
                DateTime startDate = (person.DOB < friend.DOB) ? person.DOB : friend.DOB;
                ResetRelationship(4, sourcePerson, destinationPerson, startDate, null);
            }
            // Partners
            foreach (int partnerId in person.PartnerIds)
            {
                Person partner = getPerson(partnerId);
                Person sourcePerson = (person.Id > partner.Id) ? person : partner;
                Person destinationPerson = (person.Id > partner.Id) ? partner : person;
                DateTime startDate = (person.DOB < partner.DOB) ? person.DOB : partner.DOB;
                ResetRelationship(5, sourcePerson, destinationPerson, startDate, null);
            }
            // Abusers
            foreach (int abuserId in person.AbuserIds)
            {
                Person abuser = getPerson(abuserId);
                Person sourcePerson = (person.Id > abuser.Id) ? person : abuser;
                Person destinationPerson = (person.Id > abuser.Id) ? abuser : person;
                DateTime startDate = (person.DOB < abuser.DOB) ? person.DOB : abuser.DOB;
                ResetRelationship(6, sourcePerson, destinationPerson, startDate, null);
            }
            // Victims
            foreach (int victimId in person.VictimIds)
            {
                Person victim = getPerson(victimId);
                Person sourcePerson = (person.Id > victim.Id) ? person : victim;
                Person destinationPerson = (person.Id > victim.Id) ? victim : person;
                DateTime startDate = (person.DOB < victim.DOB) ? person.DOB : victim.DOB;
                ResetRelationship(6, sourcePerson, destinationPerson, startDate, null);
            }
        }

        private void ResetRelationship(int type, Person personSource, Person personDestination, DateTime startDate, DateTime? endDate)
        {
            int Id = type * 10 ^ 6 + personSource.Id * 10 ^ 3 + personDestination.Id;
            Relationship relationship = getRelationship(Id);
            if (relationship != null)
            {
                relationship.Id = Id;
                relationship.Type = type;
                relationship.PersonSourceId = personSource.Id;
                relationship.PersonDestinationId = personDestination.Id;
                if (type < 4)
                {
                    relationship.StartDate = startDate;
                    relationship.EndDate = endDate;
                }
                relationship.Path = CreateRelationshipPath(relationship);
                relationship.PathThickness = Settings.Instance.Relationship.PathThickness;
                relationship.PathColor = Settings.Instance.Relationship.PathColor(type);
            }
            else
            {
                Relationship newRelationship = new Relationship();
                newRelationship.Id = Id;
                newRelationship.Type = type;
                newRelationship.PersonSourceId = personSource.Id;
                newRelationship.PersonDestinationId = personDestination.Id;
                newRelationship.StartDate = startDate;
                newRelationship.EndDate = endDate;
                newRelationship.Path = CreateRelationshipPath(newRelationship);
                newRelationship.PathThickness = Settings.Instance.Relationship.PathThickness;
                newRelationship.PathColor = Settings.Instance.Relationship.PathColor(type);
                Relationships.Add(newRelationship);
            }
        }

        private string CreateRelationshipPath(Relationship relationship)
        {
            string path = "";
            Person sourcePerson = getPerson(relationship.PersonSourceId);
            Person destinationPerson = getPerson(relationship.PersonDestinationId);
            Point origin = new Point(sourcePerson.X + sourcePerson.Width / 2, sourcePerson.Y + sourcePerson.Height / 2);
            Point destination = new Point(destinationPerson.X + destinationPerson.Width / 2, destinationPerson.Y + destinationPerson.Height / 2);

            bool descending = origin.Y < destination.Y;
            bool level = origin.Y == destination.Y;
            bool eastward = origin.X < destination.X;
            bool centered = origin.X == destination.X;
            double offset = Settings.Instance.Relationship.PathOffset(relationship.Type);
            double midVertical;
            double midHorizontal;
            double radius = 15;
            Point step1 = new Point();
            Point step2 = new Point();
            Point step3 = new Point();
            Point step4 = new Point();
            Point step5 = new Point();
            Point step6 = new Point();


            if (descending)
            {
                origin.Y += sourcePerson.Height / 2;
                destination.Y -= destinationPerson.Height / 2;
                midVertical = (destination.Y - origin.Y) / 2;
                step1.Y = origin.Y + midVertical - radius;
                step2.Y = step3.Y = step4.Y = step5.Y = origin.Y + midVertical;
                step6.Y = origin.Y + midVertical + radius;
            }
            else if (level)
            {
                origin.Y -= sourcePerson.Height / 2;
                destination.Y -= destinationPerson.Height / 2;
                midVertical = sourcePerson.Height / 4;
                step1.Y = step6.Y = origin.Y - midVertical + radius;
                step2.Y = step3.Y = step4.Y = step5.Y = origin.Y - midVertical;
            }
            else // ascending
            {
                origin.Y -= sourcePerson.Height / 2;
                destination.Y += destinationPerson.Height / 2;
                midVertical = (destination.Y - origin.Y) / 2;
                step1.Y = origin.Y + midVertical + radius;
                step2.Y = step3.Y = step4.Y = step5.Y = origin.Y + midVertical;
                step6.Y = origin.Y + midVertical - radius;
            }

            if (eastward)
            {
                origin.X += offset;
                destination.X -= offset;
                midHorizontal = (destination.X - origin.X) / 2;
                step1.X = step2.X = origin.X;
                step3.X = origin.X + radius;
                step4.X = destination.X - radius;
                step5.X = step6.X = destination.X;

            }
            else if (centered)
            {
                origin.X += offset;
                destination.X -= offset;
                step1.X = step2.X = step3.X = step4.X = step5.X = step6.X = origin.X;

            }
            else // westward
            {
                origin.X -= offset;
                destination.X += offset;
                midHorizontal = (destination.X - origin.X) / 2;
                step1.X = step2.X = origin.X;
                step3.X = origin.X - radius;
                step4.X = destination.X + radius;
                step5.X = step6.X = destination.X;
            }

            path = "M" + origin.ToString() + " L" + step1.ToString() + " Q" + step2.ToString() + " " + step3.ToString() + " L" + step4.ToString() + " Q" + step5.ToString() + " " + step6.ToString() + " L" + destination.ToString();
            //path = "M" + origin.ToString() + " S" + step1.ToString() + " " + step2.ToString() + " S" + step3.ToString() + " " + step4.ToString() + " T" + destination.ToString();
            //path = "M" + origin.ToString() + " L" + step1.ToString() + " L" + step2.ToString() + " L" + step3.ToString() + " L" + step4.ToString() + " L" + destination.ToString();

            return path;
        }        

        private Relationship getRelationship(int ID)
        {
            return (Relationship)relationships.Where(r => r.Id == ID).FirstOrDefault();
        }

        private Person getPerson(int ID)
        {
            return (Person)members.Where(m => m.Id == ID).FirstOrDefault();
        }

        public void InitalizePerson(Person person)
        {
            person.Id = GetNextID();
            person.FirstName = "First Name";
            person.LastName = "Last Name";
            person.Gender = "Not Specified";
            person.DOB = DateTime.Now;
            person.SiblingIds = new List<int> { };
            person.PartnerIds = new List<int> { };
            person.FriendIds = new List<int> { };
            person.ChildrenIds = new List<int> { };
            person.AbuserIds = new List<int> { };
            person.VictimIds = new List<int> { };

            person.GenerationIndex = 0;
            person.SiblingIndex = 0;

            person.Width = Settings.Instance.Person.Width;
            person.Height = Settings.Instance.Person.Height;
        }

        private int GetNextID()
        {
            if (members.Count == 0) { return 1; }
            int? maxId = members.Max(m => m.Id) + 1;
            return maxId ?? 1;
        }

        private void AddPersonToFamily(Person person)
        {
            if (Members == null)
            {
                Members = new ObservableCollection<Person> { };
            }
            Members.Add(person);
            OrderSiblings(person.GenerationIndex);
            SetTreeLayout();
        }

        private void SetPersonPosition(Person person)
        {
            person.X = Tree.Width / 2 + person.SiblingIndex * (Settings.Instance.Person.Width + Settings.Instance.Person.HorizontalSpace) - Settings.Instance.Person.Width / 2;
            person.Y = (person.GenerationIndex - Members.Min(m => m.GenerationIndex)) * (Settings.Instance.Person.Height + Settings.Instance.Person.VerticalSpace);
        }

        private void OrderSiblings(int generation)
        {
            List<Person> generationMembers = new List<Person> { };
            generationMembers = (List<Person>)members.Where(m => m.GenerationIndex == generation).OrderBy(m => m.DOB).ToList<Person>();
            // Center generation members about a zero index for easy positioning (i.e. -2,-1,0,1,2)           
            for (int i = 0; i < generationMembers.Count(); i++)
            {
                generationMembers[i].SiblingIndex = Convert.ToDouble(i) - (Convert.ToDouble(generationMembers.Count()) - 1) / 2;
            }

        }

        private void SetTreeLayout()
        {
            Tree.Width = (Members.Max(m => m.SiblingIndex) + 1 - Members.Min(m => m.SiblingIndex)) * (Settings.Instance.Person.Width + Settings.Instance.Person.HorizontalSpace);// - 40;
            Tree.Height = (Members.Max(m => m.GenerationIndex) + 1 - Members.Min(m => m.GenerationIndex)) * (Settings.Instance.Person.Height + Settings.Instance.Person.VerticalSpace);// - 30;

            foreach (Person person in Members)
            {
                SetPersonPosition(person);
            }
            ResetAllRelationships();
            SetScaledTreeDimensions();
        }

        private void SetScaledTreeDimensions()
        {
            Tree.WidthScaled = Tree.Width * Tree.Scale;
            Tree.HeightScaled = Tree.Height * Tree.Scale;
        }

        public void SetWindowSize(double width, double height)
        {
            Tree.WindowWidth = width;
            Tree.WindowHeight = height;
        }

        public void CenterTreeInWindow()
        {

            SetTreeLayout();
            Tree.XPosition = (Tree.WindowWidth / 2) - (Tree.Width / 2);
            Tree.YPosition = (Tree.WindowHeight / 2) - (Tree.Height / 2);
        }

        public void MoveTreePositionInWindow(double deltaX, double deltaY)
        {
            Tree.XPosition += deltaX; /// TreeScale;
            Tree.YPosition += deltaY; /// TreeScale;
        }

        public void ScaleTree(double scaleIncrease, double windowCenterX, double windowCenterY)

        {

            //TreeScaleOrigin = new Point(windowCenterX / windowWidth, windowCenterY / windowHeight);
            Tree.ScaleOrigin = new Point(0.5, 0.5);

            Tree.ScaleCenterX = windowCenterX;
            Tree.ScaleCenterY = windowCenterY;

            if (scaleIncrease > 0)
            { Tree.Scale = Tree.Scale * (scaleIncrease / 100); }
            else { Tree.Scale = Tree.Scale * (-100 / scaleIncrease); }

            //XPosition = XPosition - (windowWidth * TreeScale) * scaleIncrease;
            //YPosition = YPosition - (windowHeight * TreeScale)* scaleIncrease;

            SetScaledTreeDimensions();
        }

        public void Save()
        {

            SaveFileDialog savefile = new SaveFileDialog();
            // set a default file name
            savefile.FileName = "Family.fex";
            // set filters - this can be done in properties as well
            savefile.Filter = "Family Explorer files (*.fex)|*.fex|All files (*.*)|*.*";

            Nullable<bool> result = savefile.ShowDialog();

            if (result == true)
            {

                Family family = new Family();
                family.PersonSettings = Settings.Instance.Person;
                family.RelationshipSettings = Settings.Instance.Relationship;
                family.Tree = Tree;
                family.Members = Members;
                family.Relationships = Relationships;

                XmlSerializer xsSubmit = new XmlSerializer(typeof(Family));
                var subReq = family;
                using (StringWriter sww = new StringWriter())
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, subReq);
                    var xml = sww.ToString();
                    XmlDocument xdoc = new XmlDocument();
                    xdoc.LoadXml(xml);
                    xdoc.Save(savefile.FileName);
                    Title = "Family Explorer - " + savefile.FileName;
                }                
            }
        }

        public void Open()
        {
            OpenFileDialog openfile = new OpenFileDialog();
            // set a default file name
            openfile.FileName = "Family.fex";
            // set filters - this can be done in properties as well
            openfile.Filter = "Family Explorer files (*.fex)|*.fex|All files (*.*)|*.*";

            Nullable<bool> result = openfile.ShowDialog();

            if (result == true)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Family));
                using (StreamReader reader = new StreamReader(openfile.FileName))
                {
                    Family family = new Family();
                    family = (Family)serializer.Deserialize(reader);
                    if (family.PersonSettings != null) { Settings.Instance.Person = family.PersonSettings; }
                    if (family.RelationshipSettings != null) { Settings.Instance.Relationship = family.RelationshipSettings; }       
                    if (family.Tree != null) { Tree = family.Tree; }
                    if (family.Members != null) { Members = family.Members; Title = "Family Explorer - " + openfile.FileName; }
                    if (family.Relationships != null) { Relationships = family.Relationships; }
                    SetTreeLayout();
                }
            }
        }

    }
}
