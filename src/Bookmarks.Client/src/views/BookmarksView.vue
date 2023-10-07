<script setup lang="ts">
  import { useRoute } from 'vue-router'

  import { watch } from "vue";
  import { ref } from 'vue'
  import type { Ref } from 'vue'
  import type { BookmarkModel } from "@/models";
  import { api } from "@/services/api";

  const route = useRoute()

  let loading : Ref<boolean> = ref(false);
  let error : Ref<string | null> = ref(null);
  let filteredBookmarks : Ref<BookmarkModel[] | null | undefined> = ref(null);
  let tags : Ref<string[] | null> = ref(null);

  watch(
      () => route.fullPath,
      async () => {
        await fetchData();
      },
      {immediate: true}
  );

  let bookmarks : BookmarkModel[] | null = [];
  async function fetchData() {
    filteredBookmarks.value = null;
    error.value = null;
    loading.value = true;
    tags.value = null;

    try {
      bookmarks = await api.getBookmarks()
      loading.value = false;
      buildTagArray();
      search();
    } catch (err : any) {
      loading.value = false;
      error.value = err.toString();
    }
  }

  function  buildTagArray() {
    tags.value = [];
    bookmarks?.forEach((bookmark) => {
      bookmark.tags.forEach((tag) => {
        if (!tags.value?.includes(tag.name)) {
          tags.value?.push(tag.name);
        }
      });
    });

    tags.value?.sort();
  }

  let searchText = ref();
  if (route.query.q) {
    searchText.value = route.query.q;
  }

  function search() {
    if (!searchText.value) {
      filteredBookmarks.value = bookmarks;
      return;
    }

    if (searchText.value.startsWith("#")) {
      // Search by tag
      const name = searchText.value.substring(1);

      filteredBookmarks.value = bookmarks?.filter((bookmark) => {
        let foundTag = false;
        bookmark.tags.every((tag) => {
          if (tag.name == name) {
            foundTag = true;
            return false;
          }

          return true;
        });

        return foundTag;
      });

      return;
    }

    // Search by url/title/description
    const searchValue = searchText.value.toLowerCase();
    filteredBookmarks.value = bookmarks?.filter((bookmark) => {
      return bookmark.url.toLowerCase().includes(searchValue) ||
          bookmark.title?.toLowerCase().includes(searchValue) ||
          bookmark.description?.toLowerCase().includes(searchValue);
    });
  }

  function filterTag(text: string) {
    searchText.value = "#" + text;
    search();
  }
</script>

<template>
  <div class="row">
    <div class="input-group mb-3">
      <input type="text" class="form-control" placeholder="Search for bookmarks" v-model="searchText">
      <span class="input-group-text" id="basic-addon1"><button class="btn btn-sm btn-primary" type="button" @click="search">Search</button></span>
    </div>
  </div>
  <div class="row">
    <div class="col-12 col-lg-9">
      <h5>Bookmarks</h5>
      <div v-if="loading">Loading...</div>
      <div v-if="error">{{ error }}</div>
      <div class="list-group" v-if="filteredBookmarks">
        <div v-for="(item) in filteredBookmarks" class="list-group-item border-0">
          <a v-bind:href="item.url" class="fs-bold link-underline link-underline-opacity-0 link-underline-opacity-100-hover">{{ item.title ?? item.url }}</a>
          <div class="font-monospace bookmark-tag">
            <span v-for="(tag) in item.tags" class="me-1"><a href="javascript:void(0);" class="link-info link-underline link-underline-opacity-0 link-underline-opacity-100-hover" @click="filterTag(tag.name)">#{{ tag.name }}</a></span>
          </div>
          <div class="bookmark-description">
            <span class="text-truncate">{{ item.description }}</span>
            <span class="ms-1 me-1">|</span>
            <RouterLink :to="{name: 'edit', params: {id: item.id}}" class="link-success link-underline link-underline-opacity-0 link-underline-opacity-100-hover">Edit</RouterLink>
            <span class="ms-1 me-1">|</span>
            <RouterLink :to="{name: 'delete', params: {id: item.id}}" class="link-danger link-underline link-underline-opacity-0 link-underline-opacity-100-hover">Delete</RouterLink>
          </div>
        </div>
      </div>
    </div>
    <div class="col-lg-3 d-none d-lg-block">
      <h5>Tags</h5>
      <div class="d-flex flex-wrap gap-1">
        <span v-for="(item) in tags" class="list-group-item border-0">
          <a href="javascript:void(0);" @click="filterTag(item)" class="link-info link-underline link-underline-opacity-0 link-underline-opacity-100-hover">{{ item }}</a>
        </span>
      </div>
    </div>
  </div>
</template>
