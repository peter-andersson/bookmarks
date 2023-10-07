<script setup lang="ts">
  import { watch } from "vue";
  import { useRoute } from "vue-router";
  import { ref } from 'vue'
  import type { Ref } from 'vue'
  import type { Bookmark } from "@/models";

  const route = useRoute();

  let loading : Ref<boolean> = ref(false);
  let error : Ref<string | null> = ref(null);
  let bookmarks : Ref<Bookmark[] | null> = ref(null);
  let tags : Ref<string[] | null> = ref(null);

  // TODO: Search with query params
  // TODO: Cache data
  // console.log(route.query);

  watch(
      () => route.fullPath,
      async () => {
        await fetchData();
      },
      { immediate: true }
  );

  async function fetchData() {
    bookmarks.value = null;
    error.value = null;
    loading.value = true;
    tags.value = null;

    try {
      const response = await fetch('/api/bookmark');
      bookmarks.value = await response.json();
      loading.value = false;
      buildTagArray();
    } catch (err : any) {
      loading.value = false;
      error.value = err.toString();
    }
  }

  function  buildTagArray() {
    tags.value = [];
    bookmarks.value?.forEach((bookmark) => {
      bookmark.tags.forEach((tag) => {
        if (!tags.value?.includes(tag.name)) {
          tags.value?.push(tag.name);
        }
      });
    });

    tags.value?.sort();
  }
</script>

<template>
  <div class="row">
    <div class="input-group mb-3">
      <input type="text" class="form-control" placeholder="Search for bookmarks">
      <span class="input-group-text" id="basic-addon1"><button class="btn btn-sm btn-primary" type="button">Search</button></span>
    </div>
  </div>
  <div class="row">
    <div class="col-12 col-lg-9">
      <h5>Bookmarks</h5>
      <div v-if="loading">Loading...</div>
      <div v-if="error">{{ error }}</div>
      <div class="list-group" v-if="bookmarks">
        <div v-for="(item) in bookmarks" class="list-group-item border-0">
          <a v-bind:href="item.url" class="fs-bold link-underline link-underline-opacity-0 link-underline-opacity-100-hover">{{ item.title ?? item.url }}</a>
          <div class="font-monospace bookmark-tag">
            <span v-for="(tag) in item.tags" class="me-1"><a v-bind:href="'?q=' + tag.name" class="link-info link-underline link-underline-opacity-0 link-underline-opacity-100-hover">#{{ tag.name }}</a></span>
          </div>
          <div class="bookmark-description">
            <span class="text-truncate">{{ item.description }}</span>
            <span class="ms-1 me-1">|</span>
            <a v-bind:href="'edit/' + item.id" class="link-success link-underline link-underline-opacity-0 link-underline-opacity-100-hover">Edit</a>
            <span class="ms-1 me-1">|</span>
            <a v-bind:href="'delete/' + item.id" class="link-danger link-underline link-underline-opacity-0 link-underline-opacity-100-hover">Delete</a>
          </div>
        </div>
      </div>
    </div>
    <div class="col-lg-3 d-none d-lg-block">
      <h5>Tags</h5>
      <div class="d-flex flex-wrap gap-1">
        <span v-for="(item) in tags" class="list-group-item border-0">
          <a v-bind:href="'?q=' + item" class="link-info link-underline link-underline-opacity-0 link-underline-opacity-100-hover">{{ item }}</a>
        </span>
      </div>
    </div>
  </div>
</template>
